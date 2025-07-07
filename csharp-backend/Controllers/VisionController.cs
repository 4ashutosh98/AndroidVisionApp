using Microsoft.AspNetCore.Mvc;
using VisionService.Models;
using VisionService.Services;

namespace VisionService.Controllers;

[ApiController]
[Route("")]
public class VisionController : ControllerBase
{
    private readonly IVisionService _visionService;
    private readonly IParticipantMatchingService _participantMatchingService;
    private readonly ILogger<VisionController> _logger;

    public VisionController(
        IVisionService visionService, 
        IParticipantMatchingService participantMatchingService,
        ILogger<VisionController> logger)
    {
        _visionService = visionService;
        _participantMatchingService = participantMatchingService;
        _logger = logger;
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new HealthResponse());
    }

    [HttpPost("vision")]
    public async Task<IActionResult> ProcessVision([FromForm] IFormFile image)
    {
        try
        {
            var visionResult = await _visionService.ProcessVisionAsync(image, Request);
            
            if (!visionResult.Success)
            {
                return BadRequest(new { error = visionResult.Error });
            }

            // After extracting text from image, search for participant matches
            ParticipantSearchResponse? participantMatches = null;
            
            if (!string.IsNullOrWhiteSpace(visionResult.Result))
            {
                _logger.LogInformation("[Backend] Searching for participant matches for extracted text: '{Text}'", visionResult.Result);
                participantMatches = await _participantMatchingService.FindParticipantsAsync(visionResult.Result);
            }

            // Return comprehensive response with both vision results and participant matches
            var response = new
            {
                // Original vision processing results
                result = visionResult.Result,
                imageUrl = visionResult.ImageUrl,
                success = visionResult.Success,
                
                // Participant matching results
                participantMatches = participantMatches?.Success == true ? new
                {
                    success = participantMatches.Success,
                    searchTerm = participantMatches.SearchTerm,
                    totalMatches = participantMatches.TotalMatches,
                    matches = participantMatches.Matches?.Select(m => new
                    {
                        participant = new
                        {
                            id = m.Participant.Id,
                            firstName = m.Participant.FirstName,
                            lastName = m.Participant.LastName,
                            fullName = m.Participant.FullName,
                            companyName = m.Participant.CompanyName,
                            jobTitle = m.Participant.JobTitle,
                            email = m.Participant.Email
                        },
                        confidenceScore = m.ConfidenceScore,
                        matchedField = m.MatchedField
                    }).ToList(),
                    searchTimestamp = participantMatches.SearchTimestamp
                } : null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Backend] Vision endpoint error");
            return StatusCode(500, new { error = "Vision LLM processing failed" });
        }
    }

    /// <summary>
    /// Test endpoint for fuzzy participant matching without image processing
    /// </summary>
    [HttpPost("participants/search")]
    public async Task<IActionResult> SearchParticipants([FromBody] ParticipantSearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return BadRequest(new { error = "Search term is required" });
            }

            _logger.LogInformation("[Backend] Direct participant search for: '{SearchTerm}'", request.SearchTerm);
            
            var searchResult = await _participantMatchingService.FindParticipantsAsync(request.SearchTerm);
            
            return Ok(searchResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Backend] Participant search error");
            return StatusCode(500, new { error = "Participant search failed" });
        }
    }

    /// <summary>
    /// Get all participants for testing/verification
    /// </summary>
    [HttpGet("participants")]
    public async Task<IActionResult> GetAllParticipants()
    {
        try
        {
            var participants = await _participantMatchingService.GetAllParticipantsAsync();
            
            return Ok(new
            {
                success = true,
                total = participants.Count,
                participants = participants.Select(p => new
                {
                    id = p.Id,
                    firstName = p.FirstName,
                    lastName = p.LastName,
                    fullName = p.FullName,
                    companyName = p.CompanyName,
                    jobTitle = p.JobTitle,
                    email = p.Email
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Backend] Get participants error");
            return StatusCode(500, new { error = "Failed to retrieve participants" });
        }
    }

    /// <summary>
    /// Add a new participant for testing
    /// </summary>
    [HttpPost("participants")]
    public async Task<IActionResult> AddParticipant([FromBody] AddParticipantRequest request)
    {
        try
        {
            var participant = new Participant
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                CompanyName = request.CompanyName,
                JobTitle = request.JobTitle,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var addedParticipant = await _participantMatchingService.AddParticipantAsync(participant);
            
            if (addedParticipant == null)
            {
                return BadRequest(new { error = "Failed to add participant" });
            }

            return Ok(new
            {
                success = true,
                participant = new
                {
                    id = addedParticipant.Id,
                    firstName = addedParticipant.FirstName,
                    lastName = addedParticipant.LastName,
                    fullName = addedParticipant.FullName,
                    companyName = addedParticipant.CompanyName,
                    jobTitle = addedParticipant.JobTitle,
                    email = addedParticipant.Email
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Backend] Add participant error");
            return StatusCode(500, new { error = "Failed to add participant" });
        }
    }
}
