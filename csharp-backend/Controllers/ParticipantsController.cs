using Microsoft.AspNetCore.Mvc;
using VisionService.Models;
using VisionService.Services;

namespace VisionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly IParticipantMatchingService _matchingService;
    private readonly ILogger<ParticipantsController> _logger;

    public ParticipantsController(
        IParticipantMatchingService matchingService,
        ILogger<ParticipantsController> logger)
    {
        _matchingService = matchingService;
        _logger = logger;
    }

    /// <summary>
    /// Search for participants using fuzzy matching
    /// </summary>
    /// <param name="searchTerm">Name or text to search for</param>
    /// <returns>JSON response with matching participants and confidence scores</returns>
    [HttpGet("search")]
    public async Task<ActionResult<ParticipantSearchResponse>> SearchParticipants([FromQuery] string searchTerm)
    {
        _logger.LogInformation("[ParticipantsController] Search request for: '{SearchTerm}' from {RemoteIp}",
            searchTerm, Request.HttpContext.Connection.RemoteIpAddress);

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest(new ParticipantSearchResponse
            {
                Success = false,
                Error = "Search term is required",
                SearchTerm = searchTerm ?? ""
            });
        }

        var result = await _matchingService.FindParticipantsAsync(searchTerm);
        
        if (result.Success)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    /// <summary>
    /// Get all participants in the database
    /// </summary>
    /// <returns>List of all active participants</returns>
    [HttpGet]
    public async Task<ActionResult<List<Participant>>> GetAllParticipants()
    {
        _logger.LogInformation("[ParticipantsController] Get all participants request from {RemoteIp}",
            Request.HttpContext.Connection.RemoteIpAddress);

        try
        {
            var participants = await _matchingService.GetAllParticipantsAsync();
            return Ok(participants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ParticipantsController] Error getting all participants");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Add a new participant to the database
    /// </summary>
    /// <param name="participant">Participant data</param>
    /// <returns>Created participant</returns>
    [HttpPost]
    public async Task<ActionResult<Participant>> AddParticipant([FromBody] Participant participant)
    {
        _logger.LogInformation("[ParticipantsController] Add participant request: {Name} from {RemoteIp}",
            participant.FullName, Request.HttpContext.Connection.RemoteIpAddress);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _matchingService.AddParticipantAsync(participant);
            if (result != null)
            {
                return CreatedAtAction(nameof(GetAllParticipants), new { id = result.Id }, result);
            }
            else
            {
                return StatusCode(500, new { error = "Failed to create participant" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ParticipantsController] Error adding participant");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
