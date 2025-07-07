namespace VisionService.Models;

public class VisionResult
{
    public string Result { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}

public class VisionRequest
{
    public IFormFile Image { get; set; } = null!;
}

public class HealthResponse
{
    public string Status { get; set; } = "ok";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "1.0.0";
}

/// <summary>
/// Request model for participant search endpoint
/// </summary>
public class ParticipantSearchRequest
{
    public string SearchTerm { get; set; } = string.Empty;
}

/// <summary>
/// Request model for adding new participants
/// </summary>
public class AddParticipantRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
