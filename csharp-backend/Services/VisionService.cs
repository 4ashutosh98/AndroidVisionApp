using VisionService.Models;
using VisionService.Services;

namespace VisionService.Services;

public interface IVisionService
{
    Task<VisionResult> ProcessVisionAsync(IFormFile imageFile, HttpRequest request);
}

public class VisionService : IVisionService
{
    private readonly IPythonVisionProcessor _pythonProcessor;
    private readonly ILogger<VisionService> _logger;
    private readonly IConfiguration _configuration;

    public VisionService(
        IPythonVisionProcessor pythonProcessor, 
        ILogger<VisionService> logger,
        IConfiguration configuration)
    {
        _pythonProcessor = pythonProcessor;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<VisionResult> ProcessVisionAsync(IFormFile imageFile, HttpRequest request)
    {
        string? savedFilePath = null;
        
        try
        {
            _logger.LogInformation("[Backend] /vision endpoint hit from {RemoteIpAddress}", request.HttpContext.Connection.RemoteIpAddress);
            
            if (imageFile == null || imageFile.Length == 0)
            {
                _logger.LogWarning("[Backend] No image file received");
                return new VisionResult { Success = false, Error = "No image file received" };
            }

            // Save the uploaded file (replicating your Node.js multer logic)
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var uniqueFileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{imageFile.FileName}";
            savedFilePath = Path.Combine(uploadsDir, uniqueFileName);

            using (var stream = new FileStream(savedFilePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            _logger.LogInformation("[Backend] File saved at: {FilePath}", savedFilePath);

            // Create public URL for the image (replicating your Node.js static serving)
            var scheme = request.Scheme;
            var host = request.Host;
            var imageUrl = $"{scheme}://{host}/uploads/{uniqueFileName}";
            _logger.LogInformation("[Backend] Uploaded image available at: {ImageUrl}", imageUrl);

            // Convert image to base64 (replicating your Node.js logic)
            var imageBytes = await File.ReadAllBytesAsync(savedFilePath);
            var imageBase64 = Convert.ToBase64String(imageBytes);
            _logger.LogInformation("[Backend] Converted image to base64, size: {Size} bytes", imageBytes.Length);

            // Get AI provider from configuration
            var aiProvider = _configuration["Vision:AiProvider"] ?? "github";
            _logger.LogInformation("[Backend] Using AI provider: {Provider}", aiProvider);

            // Process with Python (replicating your Node.js Azure AI Inference SDK call)
            var extractedText = await _pythonProcessor.ProcessImageAsync(imageBase64, aiProvider);
            _logger.LogInformation("[Backend] Extracted text: {Text}", extractedText);

            return new VisionResult
            {
                Result = extractedText,
                ImageUrl = imageUrl,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Backend] Vision processing failed: {Error}", ex.Message);
            return new VisionResult
            {
                Success = false,
                Error = ex.Message
            };
        }
        finally
        {
            // Clean up the uploaded file (replicating your Node.js cleanup logic)
            if (!string.IsNullOrEmpty(savedFilePath) && File.Exists(savedFilePath))
            {
                try
                {
                    File.Delete(savedFilePath);
                    _logger.LogInformation("[Backend] Successfully deleted uploaded file: {FilePath}", savedFilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Backend] Failed to delete uploaded file: {FilePath}", savedFilePath);
                }
            }
        }
    }
}
