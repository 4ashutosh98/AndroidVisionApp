namespace VisionService.Configuration;

public class VisionConfiguration
{
    public string AiProvider { get; set; } = "github";
    public string? GithubToken { get; set; }
    public string? AzureAiKey { get; set; }
    public string? AzureAiEndpoint { get; set; }
    public string? AzureAiRegion { get; set; }
    public string? AzureAiModelName { get; set; }
    public string? AzureAiModelDeployment { get; set; }
    public int Port { get; set; } = 5000;
    public string PythonPath { get; set; } = @"C:\Python311\python.exe";
    public string PythonLibPath { get; set; } = @"C:\Python311\Lib\site-packages";
}
