# Python Scripts for Vision Processing

This directory contains Python scripts that handle AI-powered vision processing for the C# ASP.NET Core backend.

## Overview

The C# backend uses Python.NET to execute these Python scripts, which handle the actual communication with AI services (GitHub AI and Azure AI) for vision processing.

## Files

### Vision Processing Scripts
- **`github_vision.py`** - Handles image text extraction using GitHub AI models
- **`azure_vision.py`** - Handles image text extraction using Azure AI models

### Vision Processing Scripts
- **`github_vision.py`** - Handles text extraction using GitHub AI models
- **`azure_vision.py`** - Handles text extraction using Azure AI models

### Utility Scripts
- **`image_utils.py`** - Common image processing utilities (validation, info, resizing)

### Configuration
- **`requirements.txt`** - Python package dependencies

## Architecture

The Python scripts are designed to be:
1. **Modular** - Each script handles a specific functionality
2. **Standalone** - Can be executed independently for testing
3. **Consistent** - All scripts follow the same API pattern
4. **Error-handling** - Proper exception handling and logging

## API Pattern

All processing scripts follow this pattern:

```python
def process_image(image_base64, ...credentials):
    """
    Process image using AI service
    
    Args:
        image_base64 (str): Base64 encoded image data
        ...credentials: Service-specific credentials
        
    Returns:
        str: Extracted text from image
    """
```

## Dependencies

The scripts require these Python packages:
- `requests` - For HTTP API calls
- `Pillow` - For image processing utilities

Install with: `pip install -r requirements.txt`

## Usage from C#

The C# backend (`PythonVisionProcessor.cs`) uses Python.NET to:

1. Add this directory to Python's sys.path
2. Import the appropriate module
3. Call the `process_image` function
4. Return the result to the C# application

Example:
```csharp
// In PythonVisionProcessor.cs
dynamic visionModule = Py.Import("github_vision");
dynamic processImage = visionModule.GetAttr("process_image");
dynamic result = processImage(imageBase64, token);
```

## Testing Individual Scripts

Each script can be tested individually:

```bash
# Test GitHub vision
python github_vision.py "<base64_image>" "<github_token>"

# Test Azure vision
python azure_vision.py "<base64_image>" "<azure_endpoint>" "<azure_key>"

# Test image utilities
python image_utils.py validate "<base64_image>"
python image_utils.py info "<base64_image>"
python image_utils.py resize "<base64_image>" 800 600
```

## Error Handling

All scripts implement proper error handling:
- Errors are printed to stderr
- Exceptions are re-raised for the C# layer to handle
- HTTP errors include status codes and response text

## Configuration

The scripts use the same AI models and settings as the original Node.js backend:

### GitHub AI
- Model: `meta/Llama-3.2-11B-Vision-Instruct`
- Endpoint: `https://models.github.ai/inference/chat/completions`

### Azure AI
- Model: `gpt-4o`
- Endpoint: `{endpoint}/openai/deployments/gpt-4o/chat/completions?api-version=2024-02-15-preview`

## Message Format

The scripts use the same message format as the original Node.js implementation:

```python
messages = [
    {
        'role': 'system',
        'content': 'You are a helpful assistant...'
    },
    {
        'role': 'user',
        'content': [
            {
                'type': 'text',
                'text': 'Please extract all the text...'
            },
            {
                'type': 'image_url',
                'image_url': {
                    'url': f'data:image/jpeg;base64,{image_base64}'
                }
            }
        ]
    }
]
```

This ensures identical behavior between the Node.js and C# backends.
