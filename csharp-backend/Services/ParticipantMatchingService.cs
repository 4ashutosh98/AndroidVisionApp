using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.EntityFrameworkCore;
using VisionService.Data;
using VisionService.Models;

namespace VisionService.Services;

public interface IParticipantMatchingService
{
    Task<ParticipantSearchResponse> FindParticipantsAsync(string searchTerm);
    Task<Participant?> AddParticipantAsync(Participant participant);
    Task<List<Participant>> GetAllParticipantsAsync();
}

/// <summary>
/// Service for fuzzy matching participants against extracted text from nametags
/// 
/// FUZZY MATCHING LOGIC EXPLANATION:
/// 
/// This service uses multiple fuzzy matching algorithms to find the best participant matches:
/// 
/// 1. LEVENSHTEIN DISTANCE (Edit Distance):
///    - Measures minimum number of single-character edits (insertions, deletions, substitutions)
///    - Score: 0-100 (100 = perfect match)
///    - Good for: Typos, OCR errors, slight misspellings
///    - Example: "John Smith" vs "Jon Smith" = 94% match
/// 
/// 2. TOKEN SET RATIO:
///    - Compares sets of words regardless of order
///    - Handles partial matches and word reordering
///    - Example: "Smith, John" vs "John Smith" = 100% match
/// 
/// 3. PARTIAL RATIO:
///    - Finds best matching substring
///    - Good for extracting names from longer text
///    - Example: "Dr. John Smith PhD" vs "John Smith" = 100% match
/// 
/// 4. TOKEN SORT RATIO:
///    - Sorts words alphabetically then compares
///    - Handles word order differences
///    - Example: "Smith John" vs "John Smith" = 100% match
/// 
/// CONFIDENCE SCORING:
/// - We take the HIGHEST score from all algorithms for each field
/// - Minimum threshold: 60% (configurable)
/// - Multiple field matching: Boost score if both first+last name match
/// - Results sorted by confidence score (highest first)
/// - Return top 5 matches above threshold
/// 
/// SEARCH FIELDS:
/// - Full Name (FirstName + LastName)
/// - First Name only
/// - Last Name only  
/// - Company Name
/// - This covers most nametag scenarios
/// </summary>
public class ParticipantMatchingService : IParticipantMatchingService
{
    private readonly ParticipantDbContext _context;
    private readonly ILogger<ParticipantMatchingService> _logger;
    
    // Fuzzy matching configuration
    private const int MinimumConfidenceThreshold = 60; // 60% minimum match
    private const int MaxResults = 5; // Return top 5 matches
    private const int FullNameBonus = 10; // Bonus points for full name matches

    public ParticipantMatchingService(
        ParticipantDbContext context,
        ILogger<ParticipantMatchingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Finds participant matches using fuzzy string matching algorithms
    /// </summary>
    public async Task<ParticipantSearchResponse> FindParticipantsAsync(string searchTerm)
    {
        try
        {
            _logger.LogInformation("[ParticipantMatching] Searching for: '{SearchTerm}'", searchTerm);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ParticipantSearchResponse
                {
                    Success = false,
                    Error = "Search term cannot be empty",
                    SearchTerm = searchTerm
                };
            }

            // Get all active participants from database
            var allParticipants = await _context.Participants
                .Where(p => p.IsActive)
                .ToListAsync();

            _logger.LogInformation("[ParticipantMatching] Loaded {Count} participants from database", allParticipants.Count);

            // Clean and normalize search term
            var cleanSearchTerm = CleanText(searchTerm);
            _logger.LogInformation("[ParticipantMatching] Cleaned search term: '{CleanTerm}'", cleanSearchTerm);

            // Find matches using fuzzy algorithms
            var matches = new List<ParticipantMatch>();

            foreach (var participant in allParticipants)
            {
                var match = CalculateBestMatch(participant, cleanSearchTerm);
                if (match.ConfidenceScore >= MinimumConfidenceThreshold)
                {
                    matches.Add(match);
                }
            }

            // Sort by confidence score (highest first) and take top N
            var topMatches = matches
                .OrderByDescending(m => m.ConfidenceScore)
                .Take(MaxResults)
                .ToList();

            _logger.LogInformation("[ParticipantMatching] Found {TotalMatches} matches above {Threshold}% threshold, returning top {MaxResults}",
                matches.Count, MinimumConfidenceThreshold, MaxResults);

            foreach (var match in topMatches)
            {
                _logger.LogInformation("[ParticipantMatching] Match: {Name} ({Score}% confidence, matched on {Field})",
                    match.Participant.FullName, match.ConfidenceScore, match.MatchedField);
            }

            return new ParticipantSearchResponse
            {
                Success = true,
                SearchTerm = searchTerm,
                TotalMatches = matches.Count,
                Matches = topMatches
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ParticipantMatching] Error during participant search");
            return new ParticipantSearchResponse
            {
                Success = false,
                Error = ex.Message,
                SearchTerm = searchTerm
            };
        }
    }

    /// <summary>
    /// Calculates the best fuzzy match score for a participant against search term
    /// </summary>
    private ParticipantMatch CalculateBestMatch(Participant participant, string searchTerm)
    {
        var bestScore = 0;
        var bestField = "";

        // Test against different participant fields
        var candidates = new Dictionary<string, string>
        {
            ["Full Name"] = CleanText(participant.FullName),
            ["First Name"] = CleanText(participant.FirstName),
            ["Last Name"] = CleanText(participant.LastName),
            ["Company"] = CleanText(participant.CompanyName ?? "")
        };

        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate.Value)) continue;

            var score = CalculateFuzzyScore(searchTerm, candidate.Value);
            
            // Bonus for full name matches (more comprehensive)
            if (candidate.Key == "Full Name" && score > 0)
            {
                score = Math.Min(100, score + FullNameBonus);
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestField = candidate.Key;
            }
        }

        return new ParticipantMatch
        {
            Participant = participant,
            ConfidenceScore = bestScore,
            MatchedField = bestField,
            SearchTerm = searchTerm
        };
    }

    /// <summary>
    /// Calculates fuzzy match score using multiple algorithms and returns the highest score
    /// </summary>
    private int CalculateFuzzyScore(string search, string target)
    {
        if (string.IsNullOrWhiteSpace(search) || string.IsNullOrWhiteSpace(target))
            return 0;

        // Try different fuzzy matching algorithms and take the best score
        var scores = new[]
        {
            Fuzz.Ratio(search, target),              // Basic Levenshtein distance
            Fuzz.PartialRatio(search, target),       // Best matching substring
            Fuzz.TokenSortRatio(search, target),     // Sorted words comparison
            Fuzz.TokenSetRatio(search, target),      // Set-based comparison
            Fuzz.WeightedRatio(search, target)       // Weighted combination of above
        };

        var bestScore = scores.Max();
        
        // Additional boost for exact case-insensitive matches
        if (string.Equals(search, target, StringComparison.OrdinalIgnoreCase))
        {
            bestScore = 100;
        }

        return bestScore;
    }

    /// <summary>
    /// Cleans and normalizes text for better matching
    /// </summary>
    private string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";

        return text
            .Trim()
            .ToLowerInvariant()
            // Remove common prefixes/suffixes that might appear on nametags
            .Replace("dr.", "").Replace("prof.", "").Replace("mr.", "").Replace("ms.", "").Replace("mrs.", "")
            .Replace("phd", "").Replace("md", "").Replace("jr.", "").Replace("sr.", "")
            // Remove extra whitespace
            .Replace("  ", " ").Trim();
    }

    /// <summary>
    /// Adds a new participant to the database
    /// </summary>
    public async Task<Participant?> AddParticipantAsync(Participant participant)
    {
        try
        {
            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            _logger.LogInformation("[ParticipantMatching] Added new participant: {Name}", participant.FullName);
            return participant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ParticipantMatching] Error adding participant: {Name}", participant.FullName);
            return null;
        }
    }

    /// <summary>
    /// Gets all participants from the database
    /// </summary>
    public async Task<List<Participant>> GetAllParticipantsAsync()
    {
        return await _context.Participants
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }
}
