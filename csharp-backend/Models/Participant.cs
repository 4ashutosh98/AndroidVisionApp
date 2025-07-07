using System.ComponentModel.DataAnnotations;

namespace VisionService.Models;

/// <summary>
/// Participant entity representing a registered event participant
/// </summary>
public class Participant
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string FullName => $"{FirstName} {LastName}";
    
    [MaxLength(100)]
    public string? CompanyName { get; set; }
    
    [MaxLength(100)]
    public string? JobTitle { get; set; }
    
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Represents a fuzzy match result with confidence score
/// </summary>
public class ParticipantMatch
{
    public Participant Participant { get; set; } = null!;
    public int ConfidenceScore { get; set; }
    public string MatchedField { get; set; } = string.Empty;
    public string SearchTerm { get; set; } = string.Empty;
}

/// <summary>
/// Response model for participant search API
/// </summary>
public class ParticipantSearchResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public int TotalMatches { get; set; }
    public List<ParticipantMatch> Matches { get; set; } = new();
    public DateTime SearchTimestamp { get; set; } = DateTime.UtcNow;
}
