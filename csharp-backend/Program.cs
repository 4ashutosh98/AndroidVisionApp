using Python.Runtime;
using Serilog;
using VisionService.Services;
using VisionService.Configuration;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure the app to use the port from configuration and listen on all interfaces
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/vision-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for React Native app
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register custom services
builder.Services.Configure<VisionConfiguration>(builder.Configuration.GetSection("Vision"));

// Override with environment variables if present
builder.Services.PostConfigure<VisionConfiguration>(config =>
{
    config.AiProvider = Environment.GetEnvironmentVariable("AI_PROVIDER") ?? config.AiProvider;
    config.GithubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? config.GithubToken;
    config.AzureAiKey = Environment.GetEnvironmentVariable("AZURE_AI_KEY") ?? config.AzureAiKey;
    config.AzureAiEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT") ?? config.AzureAiEndpoint;
    config.AzureAiRegion = Environment.GetEnvironmentVariable("AZURE_AI_REGION") ?? config.AzureAiRegion;
    config.AzureAiModelName = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_NAME") ?? config.AzureAiModelName;
    config.AzureAiModelDeployment = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT") ?? config.AzureAiModelDeployment;
    
    if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out int port))
    {
        config.Port = port;
    }
});

builder.Services.AddSingleton<IPythonVisionProcessor, PythonVisionProcessor>();
builder.Services.AddScoped<IVisionService, VisionService.Services.VisionService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRouting();

// Serve static files from uploads directory
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
    Log.Information("[Backend] Created uploads directory: {UploadsPath}", uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.MapControllers();

// Ensure to dispose the python engine on shutdown
AppDomain.CurrentDomain.ProcessExit += (s, e) => PythonEngine.Shutdown();

Log.Information("[Backend] Starting Vision Service on port {Port}", builder.Configuration["PORT"] ?? "5000");

app.Run();
