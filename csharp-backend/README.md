# C# Vision Service

This C# implementation provides AI-powered text extraction from images using Python.NET to execute Python code for Vision LLM processing.

## Architecture

- **C# ASP.NET Core**: Main web service framework
- **Python.NET**: Executes Python code for AI processing
- **Azure AI Inference SDK**: Via Python, maintains compatibility with both GitHub AI and Azure AI
- **Serilog**: Structured logging identical to your Node.js console.log statements

## Setup Instructions

### 1. Prerequisites

- **.NET 8.0 SDK**: Download from https://dotnet.microsoft.com/download
- **Python 3.11**: Download from https://python.org/downloads
- **Python packages**: Will be auto-installed by the service

### 2. Configure Python Path

Update `appsettings.json` with your Python installation paths:

```json
{
  "Vision": {
    "PythonPath": "C:\\Python311\\python.exe",
    "PythonLibPath": "C:\\Python311\\Lib\\site-packages"
  }
}
```

### 3. Configure AI Provider

#### For GitHub AI (Development):
```json
{
  "Vision": {
    "AiProvider": "github",
    "GithubToken": "your_github_token_here"
  }
}
```

#### For Azure AI (Production):
```json
{
  "Vision": {
    "AiProvider": "azure",
    "AzureAiKey": "your_azure_key_here",
    "AzureAiEndpoint": "https://your-service.cognitiveservices.azure.com/",
    "AzureAiModelName": "gpt-4o"
  }
}
```

### 4. Run the Service

```bash
cd csharp-backend
dotnet restore
dotnet run
```

The service will start on http://localhost:5000

## API Endpoints

All endpoints are identical to your Node.js implementation:

- `GET /health` - Health check
- `POST /vision` - Vision LLM processing

## Features Replicated

✅ **Exact same API responses** as Node.js version  
✅ **Identical logging format** with [Backend] prefix  
✅ **Same file upload/cleanup logic** using multer-equivalent  
✅ **Static file serving** for /uploads directory  
✅ **CORS enabled** for React Native frontend  
✅ **GitHub AI and Azure AI support** via Python  
✅ **Error handling and cleanup** matching Node.js behavior  

## Python Code Execution

The service executes Python scripts that replicate your exact Node.js Azure AI Inference SDK calls:

```python
# GitHub AI processing (identical to your Node.js)
messages = [
    {
        'role': 'system',
        'content': 'You are a helpful assistant that can read and extract text from images using advanced vision understanding.'
    },
    {
        'role': 'user',
        'content': [
            {
                'type': 'text',
                'text': 'Please extract all the text visible in this image. Only return the text content, nothing else.'
            },
            {
                'type': 'image_url',
                'image_url': {
                    'url': data_url
                }
            }
        ]
    }
]
```

## Migration Benefits

1. **Drop-in Replacement**: Same API, same responses
2. **C# Integration**: Now part of your C# ecosystem
3. **Maintained Logic**: All vision processing logic preserved
4. **Azure Ready**: Easy integration with Azure services
5. **Logging**: Structured logging with Serilog
6. **Configuration**: Standard .NET configuration system

## Frontend Integration

No changes needed to your React Native frontend! Update the `BACKEND_URL` in `src/App.tsx`:

```typescript
const BACKEND_URL = 'http://YOUR_IP:5000';  // Changed port from 3000 to 5000
```

## Production Deployment

For Azure deployment, this C# service can be easily deployed to:
- Azure App Service
- Azure Container Apps
- Azure Kubernetes Service

The Python.NET integration ensures your vision processing logic works identically in any environment.
