using Microsoft.AspNetCore.Mvc;
using VisionService.Models;
using VisionService.Services;

namespace VisionService.Controllers;

[ApiController]
[Route("")]
public class VisionController : ControllerBase
{
    private readonly IVisionService _visionService;
    private readonly ILogger<VisionController> _logger;

    public VisionController(IVisionService visionService, ILogger<VisionController> logger)
    {
        _visionService = visionService;
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
            var result = await _visionService.ProcessVisionAsync(image, Request);
            
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { result = result.Result, imageUrl = result.ImageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Backend] Vision endpoint error");
            return StatusCode(500, new { error = "Vision LLM processing failed" });
        }
    }
}
