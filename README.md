# Vision AI App

A **React Web Application** with C# backend for vision-based text extraction using GitHub AI Vision models. This application allows users to take photos via webcam or upload images and extract text using advanced AI vision capabilities.

> 🚀 **Quick Start**: Run `quick-start.bat` to start both backend and frontend immediately!

## 🏗️ Architecture

### Frontend
- **React Web App** (TypeScript) - Works on any device with a browser
- **Webcam Integration** - Camera access for real-time photo capture
- **File Upload** - Support for uploading existing images
- **Responsive Design** - Mobile-friendly interface
- **Axios** - HTTP client for API communication

### Backend
- **C# .NET 8.0** - High-performance web API
- **ASP.NET Core** - Web framework
- **Python.NET** - Python integration for AI processing
- **Serilog** - Structured logging
- **DotNetEnv** - Environment variable management

### AI Processing
- **GitHub AI** (Primary) - Meta Llama-3.2-11B-Vision-Instruct model
- **Azure AI** (Alternative) - Azure OpenAI vision models
- **Python Scripts** - Image processing and AI integration

## 📋 Prerequisites

### System Requirements
- **Windows 10/11** (for development)
- **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Python 3.11** (via Conda/Anaconda)
- **Node.js 18+** (for React web app)
- **Git** for version control
- **Modern web browser** (Chrome, Firefox, Safari, Edge)

## 🚀 Quick Start

⚡ **Super Fast Setup**: Just run this command and you're done!
```bash
quick-start.bat
```
This will automatically:
1. Install all dependencies
2. Start the C# backend
3. Start the React web frontend
4. Open the app in your browser

### Manual Setup (if you prefer step-by-step)

### 1. Clone the Repository
```bash
git clone <repository-url>
cd AndroidVisionApp
```

### 2. Set Up Python Environment
```bash
# Create conda environment
conda create -n confirmed_vision_app python=3.11
conda activate confirmed_vision_app

# Install Python dependencies
cd csharp-backend/python_scripts
pip install -r requirements.txt
```

### 3. Configure Environment Variables
Create `csharp-backend/.env` file:
```env
# GitHub AI Configuration (Development)
GITHUB_TOKEN=your_github_token_here

# Azure AI Configuration (Production) - Optional
AZURE_AI_KEY=
AZURE_AI_ENDPOINT=
AZURE_AI_REGION=
AZURE_AI_MODEL_NAME=gpt-4o
AZURE_AI_MODEL_DEPLOYMENT=

# AI Provider Selection
AI_PROVIDER=github
```

⚠️ **Important**: Never commit the `.env` file to version control!

### 4. Update Backend Configuration
Edit `csharp-backend/appsettings.json` and update the Python paths:
```json
{
  "Vision": {
    "AiProvider": "github",
    "PythonPath": "C:\\Users\\YourUsername\\.conda\\envs\\confirmed_vision_app\\python.exe",
    "PythonLibPath": "C:\\Users\\YourUsername\\.conda\\envs\\confirmed_vision_app\\Lib\\site-packages"
  }
}
```

### 5. Start the C# Backend
```bash
cd csharp-backend
dotnet restore
dotnet build
dotnet run
```
Backend will start on `http://localhost:5000`

### 6. Start the React Web Frontend
```bash
# In a new terminal
cd web-frontend
npm install
npm start
```
Web app will open automatically at `http://localhost:3000`

## 🔧 Detailed Setup Instructions

### Web Application Setup

The application runs as a web app that works on all devices:

1. **Camera Access**: 
   - Modern browsers can access device cameras
   - On mobile, choose "Use Camera" for direct camera access
   - HTTPS required for camera (works on localhost for development)

2. **File Upload**:
   - Upload existing images from device gallery
   - Drag & drop support on desktop
   - Mobile: "Choose Image or Take Photo" opens camera app

3. **Cross-Platform**:
   - Works on any device with a web browser
   - Responsive design for mobile and desktop
   - No app store approval needed - deploy immediately!

## 📁 Project Structure

```
AndroidVisionApp/
├── web-frontend/                 # React Web Application
│   ├── public/                   # Static files
│   ├── src/                      # Web app source code
│   │   ├── App.tsx               # Main web app component
│   │   ├── index.tsx             # App entry point
│   │   └── index.css             # Styling
│   ├── package.json              # Web app dependencies
│   └── tsconfig.json             # TypeScript configuration
├── csharp-backend/               # C# backend API
│   ├── Controllers/              # API controllers
│   │   └── VisionController.cs   # Vision processing endpoints
│   ├── Services/                 # Business logic
│   │   ├── VisionService.cs      # Main vision service
│   │   └── PythonVisionProcessor.cs # Python integration
│   ├── Models/                   # Data models
│   │   ├── VisionResult.cs       # API response models
│   │   └── HealthResponse.cs     # Health check model
│   ├── Configuration/            # Configuration classes
│   │   └── VisionConfiguration.cs # App settings
│   ├── python_scripts/           # Python AI processing
│   │   ├── github_vision.py      # GitHub AI integration
│   │   ├── azure_vision.py       # Azure AI integration
│   │   ├── image_utils.py        # Image utilities
│   │   └── requirements.txt      # Python dependencies
│   ├── Program.cs                # Application entry point
│   ├── appsettings.json          # Configuration file
│   ├── .env                      # Environment variables (not in git)
│   └── VisionService.csproj      # Project file
├── src/                          # Legacy React Native code (kept for reference)
├── quick-start.bat               # One-click setup script
├── package.json                  # Legacy RN dependencies
├── .gitignore                    # Git ignore rules
└── README.md                     # This file
```

## 💡 Code Logic Overview

### React Web Frontend Flow

1. **App Initialization** (`web-frontend/src/App.tsx`):
   - Performs health check with backend
   - Displays connection status
   - Provides camera and file upload options

2. **Image Capture Methods**:
   - **Webcam**: Uses `react-webcam` for real-time camera access
   - **File Upload**: HTML5 file input with camera capture support
   - **Mobile Optimized**: Automatically uses rear camera on mobile devices

3. **Image Processing**:
   - Converts captured/selected images to FormData
   - Sends multipart/form-data to `/vision` endpoint
   - Displays loading indicator during AI processing
   - Shows extracted text results in formatted display

### C# Backend Flow
(Same as before - no changes needed to backend)

1. **Program.cs**:
   - Configures services and middleware
   - Sets up CORS for mobile app
   - Initializes Python integration
   - Configures logging with Serilog

2. **VisionController.cs**:
   - Handles `/health` endpoint for connectivity checks
   - Processes `/vision` POST requests
   - Manages file uploads and cleanup

3. **VisionService.cs**:
   - Main business logic for vision processing
   - Handles file I/O operations
   - Manages temporary file cleanup
   - Coordinates with Python processor

4. **PythonVisionProcessor.cs**:
   - Initializes Python runtime
   - Loads Python scripts dynamically
   - Executes AI vision processing
   - Manages Python environment and packages

### Python AI Processing

1. **github_vision.py**:
   - Interfaces with GitHub AI models API
   - Uses Meta Llama-3.2-11B-Vision-Instruct
   - Processes base64 encoded images
   - Returns extracted text

2. **azure_vision.py**:
   - Alternative Azure AI integration
   - Uses Azure OpenAI vision models
   - Fallback option for production

3. **image_utils.py**:
   - Image validation and processing utilities
   - Base64 encoding/decoding
   - Image format verification

## 🌐 API Endpoints

### Health Check
```
GET /health
Response: {"status":"ok","timestamp":"2025-01-01T00:00:00Z","version":"1.0.0"}
```

### Vision Processing
```
POST /vision
Content-Type: multipart/form-data
Body: FormData with 'image' field
Response: {"result":"extracted text","imageUrl":"temp_url","success":true}
```

## 🔐 Security Features

- **Environment Variables**: Sensitive data stored in `.env` files
- **CORS Configuration**: Controlled cross-origin access
- **File Cleanup**: Automatic deletion of uploaded files
- **Input Validation**: Image file validation and size limits
- **Error Handling**: Comprehensive error logging and user feedback

## 🛠️ Development Workflow

### Frontend Development
```bash
# Navigate to web frontend
cd web-frontend

# Install dependencies
npm install

# Start development server with hot reload
npm start

# Build for production
npm run build
```

### Backend Development
```bash
# Navigate to backend
cd csharp-backend

# Build and run
dotnet build
dotnet run

# Run with hot reload
dotnet watch run
```

### Python Development
```bash
# Activate environment
conda activate confirmed_vision_app

# Test Python scripts directly
cd csharp-backend/python_scripts
python github_vision.py <base64_image> <github_token>
```

## 🐛 Troubleshooting

### Common Issues

1. **"Backend: Error" in Web App**:
   - Check if C# backend is running: `dotnet run` in `csharp-backend/` folder
   - Verify backend URL in `web-frontend/src/App.tsx`: `const BACKEND_URL = 'http://localhost:5000'`
   - Ensure Windows Firewall allows port 5000
   - Try accessing `http://localhost:5000/health` directly in browser

2. **Camera Not Working in Browser**:
   - Ensure you're using HTTPS or localhost (required for camera access)
   - Grant camera permissions when prompted
   - Try switching between "Use Camera" and "Upload File" modes
   - On mobile: use "Choose Image or Take Photo" for best experience

3. **Python Integration Errors**:
   - Activate conda environment: `conda activate confirmed_vision_app`
   - Check Python path in `appsettings.json` matches your conda environment
   - Verify Python packages installed: `pip list` (should show requests, pillow, etc.)
   - Check Python scripts are in `csharp-backend/python_scripts/` directory

4. **"GitHub API error" Messages**:
   - Verify GitHub token is valid and not expired
   - Check token has necessary permissions
   - Ensure `.env` file is in `csharp-backend/` directory
   - Test token manually: visit [GitHub AI Models](https://github.com/marketplace/models)

5. **Web App Build/Deployment Issues**:
   - Clear npm cache: `npm start -- --reset-cache`
   - Delete node_modules and reinstall: `rm -rf node_modules && npm install`
   - Check for TypeScript errors: `npm run build`
   - Ensure all dependencies are compatible

5. **"Address already in use" Error**:
   - Kill existing processes: `Get-Process -Name "dotnet" | Stop-Process -Force`
   - Change port in `appsettings.json` if needed
   - Restart your terminal/command prompt

### Debug Commands

```bash
# Check if backend is running
curl http://localhost:5000/health
# Should return: {"status":"ok","timestamp":"...","version":"1.0.0"}

# Check web app in browser
# Open: http://localhost:3000

# View backend logs (run in csharp-backend directory)
dotnet run
# Watch for any error messages

# Check browser console for frontend errors
# Press F12 in browser, check Console tab

# Check Python environment
conda activate confirmed_vision_app
python --version
pip list | grep -E "(requests|pillow)"

# Test Python script directly
cd csharp-backend/python_scripts
python github_vision.py "base64_image_here" "your_github_token"

# Clear React cache if issues
cd web-frontend
npm start -- --reset-cache
```

## 📝 Environment Variables

### Required Variables (.env file)
```env
GITHUB_TOKEN=your_github_token_here
AI_PROVIDER=github
```

### Optional Variables
```env
AZURE_AI_KEY=your_azure_key
AZURE_AI_ENDPOINT=your_azure_endpoint
AZURE_AI_REGION=your_azure_region
PORT=5000
```

## 🎯 Features

- ✅ **Web-Based**: Works on any device with a browser - no app store needed!
- ✅ **Camera Integration**: Real-time camera access through web browser
- ✅ **File Upload**: Upload existing images or use mobile camera
- ✅ **Vision AI Processing**: Advanced text extraction using GitHub AI (Meta Llama-3.2-11B-Vision-Instruct)
- ✅ **Mobile Optimized**: Responsive design works perfectly on phones and tablets
- ✅ **Instant Deployment**: Deploy to web server and go live immediately
- ✅ **High-Performance Backend**: C# .NET 8.0 with Python AI integration
- ✅ **Dual AI Support**: GitHub AI (primary) + Azure AI (fallback)
- ✅ **Error Handling**: Comprehensive error handling and user feedback
- ✅ **Structured Logging**: Detailed logging with Serilog for debugging
- ✅ **Environment Configuration**: Secure credential management with .env files
- ✅ **File Management**: Automatic cleanup of temporary uploaded images
- ✅ **Cross-Platform**: Windows, Mac, Linux, Android, iOS - works everywhere!

## 🔄 Version History

- **v1.0.0**: Initial release with GitHub AI integration
- **v0.9.0**: Migrated from Node.js to C# backend
- **v0.8.0**: Added Python AI processing integration
- **v0.7.0**: Implemented React Native frontend

## 📄 License

This project is licensed under the MIT License. See LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 🆘 Support

For issues and questions:
1. Check this README for troubleshooting
2. Review the logs for detailed error messages
3. Create an issue in the repository
4. Provide logs and system information when reporting issues

---

**Happy coding! 🚀**
