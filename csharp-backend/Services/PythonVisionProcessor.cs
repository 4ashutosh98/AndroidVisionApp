using Python.Runtime;
using VisionService.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace VisionService.Services;

public interface IPythonVisionProcessor : IDisposable
{
    Task<string> ProcessImageAsync(string imageBase64, string aiProvider);
    Task<bool> ValidateImageAsync(string imageBase64);
    Task<dynamic> GetImageInfoAsync(string imageBase64);
    bool IsInitialized { get; }
}

public class PythonVisionProcessor : IPythonVisionProcessor
{
    private readonly VisionConfiguration _config;
    private readonly ILogger<PythonVisionProcessor> _logger;
    private readonly string _pythonScriptsPath;
    private bool _pythonInitialized = false;
    private readonly object _initLock = new object();

    public bool IsInitialized => _pythonInitialized;

    public PythonVisionProcessor(IOptions<VisionConfiguration> config, ILogger<PythonVisionProcessor> logger)
    {
        _config = config.Value;
        _logger = logger;
        _pythonScriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "python_scripts");
        InitializePython();
    }

    private void InitializePython()
    {
        lock (_initLock)
        {
            if (_pythonInitialized) return;

            try
            {
                // Verify Python scripts directory exists
                if (!Directory.Exists(_pythonScriptsPath))
                {
                    throw new DirectoryNotFoundException($"Python scripts directory not found: {_pythonScriptsPath}");
                }

                // Verify all required Python files exist
                var requiredFiles = new[] { "github_vision.py", "azure_vision.py", "image_utils.py" };
                foreach (var file in requiredFiles)
                {
                    var filePath = Path.Combine(_pythonScriptsPath, file);
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"Required Python script not found: {filePath}");
                    }
                }

                // Set Python DLL path if configured
                if (!string.IsNullOrEmpty(_config.PythonPath))
                {
                    var pythonDir = Path.GetDirectoryName(_config.PythonPath);
                    var pythonDll = Path.Combine(pythonDir!, "python311.dll");
                    if (File.Exists(pythonDll))
                    {
                        Runtime.PythonDLL = pythonDll;
                    }
                }

                // Initialize Python engine
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

                // Add site-packages to Python path if configured
                if (!string.IsNullOrEmpty(_config.PythonLibPath) && Directory.Exists(_config.PythonLibPath))
                {
                    using (Py.GIL())
                    {
                        dynamic sys = Py.Import("sys");
                        sys.path.append(_config.PythonLibPath);
                        _logger.LogInformation("[Backend] Added Python lib path: {Path}", _config.PythonLibPath);
                    }
                }

                _logger.LogInformation("[Backend] Python engine initialized successfully");
                _logger.LogInformation("[Backend] Python scripts path: {Path}", _pythonScriptsPath);

                // Note: Python packages should be pre-installed in the conda environment
                // to avoid port conflicts and restart loops during runtime initialization

                _pythonInitialized = true;
                _logger.LogInformation("[Backend] Python vision processor ready");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Backend] Failed to initialize Python engine");
                throw;
            }
        }
    }

    public async Task<string> ProcessImageAsync(string imageBase64, string aiProvider)
    {
        if (!_pythonInitialized)
        {
            throw new InvalidOperationException("Python engine not initialized");
        }

        return await Task.Run(() =>
        {
            using (Py.GIL())
            {
                try
                {
                    // Add the python_scripts directory to Python path
                    dynamic sys = Py.Import("sys");
                    
                    // Simply append the path - Python will handle duplicates
                    sys.path.insert(0, _pythonScriptsPath);

                    // Import the appropriate module
                    string moduleName = aiProvider.ToLower() == "github" ? "github_vision" : "azure_vision";
                    dynamic visionModule = Py.Import(moduleName);
                    
                    // Get the process_image function
                    dynamic processImage = visionModule.GetAttr("process_image");
                    
                    // Call the function based on provider
                    dynamic result;
                    if (aiProvider.ToLower() == "github")
                    {
                        var token = _config.GithubToken ?? throw new InvalidOperationException("GitHub token not configured");
                        result = processImage(imageBase64, token);
                    }
                    else
                    {
                        var endpoint = _config.AzureAiEndpoint ?? throw new InvalidOperationException("Azure AI endpoint not configured");
                        var key = _config.AzureAiKey ?? throw new InvalidOperationException("Azure AI key not configured");
                        result = processImage(imageBase64, endpoint, key);
                    }
                    
                    _logger.LogInformation("[Backend] Python vision processing completed successfully using {Provider}", aiProvider);
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Backend] Python vision processing failed with provider {Provider}", aiProvider);
                    throw;
                }
            }
        });
    }

    public async Task<bool> ValidateImageAsync(string imageBase64)
    {
        if (!_pythonInitialized)
        {
            throw new InvalidOperationException("Python engine not initialized");
        }

        return await Task.Run(() =>
        {
            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.insert(0, _pythonScriptsPath);

                    dynamic imageUtils = Py.Import("image_utils");
                    dynamic validateImage = imageUtils.GetAttr("validate_image_base64");
                    
                    dynamic result = validateImage(imageBase64);
                    return result.As<bool>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Backend] Error validating image with Python");
                    return false;
                }
            }
        });
    }

    public async Task<dynamic> GetImageInfoAsync(string imageBase64)
    {
        if (!_pythonInitialized)
        {
            throw new InvalidOperationException("Python engine not initialized");
        }

        return await Task.Run(() =>
        {
            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.insert(0, _pythonScriptsPath);

                    dynamic imageUtils = Py.Import("image_utils");
                    dynamic getImageInfo = imageUtils.GetAttr("get_image_info");
                    
                    return getImageInfo(imageBase64);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Backend] Error getting image info with Python");
                    throw;
                }
            }
        });
    }

    public void Dispose()
    {
        if (_pythonInitialized)
        {
            try
            {
                PythonEngine.Shutdown();
                _logger.LogInformation("[Backend] Python engine shut down successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Backend] Error shutting down Python engine");
            }
        }
    }
}
