# Android Vision App

A React Native mobile application with C# backend for vision-based text extraction using GitHub AI Vision models. This application allows users to take photos and extract text using advanced AI vision capabilities.

## 🏗️ Architecture

### Frontend
- **React Native** (v0.80.0) - Cross-platform mobile app
- **TypeScript** - Type-safe development
- **Axios** - HTTP client for API communication
- **React Native Image Picker** - Camera and gallery integration

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
- **Node.js 18+** (for React Native)
- **Android Studio** (for Android development)
- **Git** for version control

### Mobile Development
- **Android device** with USB debugging enabled
- **USB cable** for device connection
- **WiFi network** (both PC and phone on same network)

## 🚀 Quick Start

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

### 4. Update Configuration
Edit `csharp-backend/appsettings.json`:
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
Backend will start on `http://0.0.0.0:5000`

### 6. Configure React Native App
Find your PC's IP address:
```bash
ipconfig
```
Look for IPv4 Address under your WiFi adapter (e.g., 192.168.1.100)

Update `src/App.tsx`:
```typescript
const BACKEND_URL = 'http://YOUR_PC_IP:5000'; // Replace with your IP
```

### 7. Start React Native App
```bash
# Install dependencies
npm install

# Start Metro bundler
npm start

# In another terminal, build and run on Android
npx react-native run-android
```

## 🔧 Detailed Setup Instructions

### Python Environment Setup

The application uses Python for AI vision processing. Here's how to set it up:

1. **Install Conda/Anaconda** if not already installed
2. **Create dedicated environment**:
   ```bash
   conda create -n confirmed_vision_app python=3.11
   conda activate confirmed_vision_app
   ```
3. **Install required packages**:
   ```bash
   cd csharp-backend/python_scripts
   pip install -r requirements.txt
   ```

### GitHub AI Token Setup

1. **Get GitHub Token**:
   - Go to [GitHub Settings > Developer settings > Personal access tokens](https://github.com/settings/tokens)
   - Create a new token with appropriate permissions
   - Copy the token

2. **Configure Token**:
   - Add to `csharp-backend/.env` file
   - Never commit this file to version control

### Network Configuration

For the mobile app to communicate with the backend:

1. **Find your PC's IP address**:
   ```bash
   ipconfig
   ```
2. **Ensure both devices are on same WiFi**
3. **Update the React Native app** with your PC's IP
4. **Configure Windows Firewall** to allow connections on port 5000

### Android Development Setup

1. **Install Android Studio**
2. **Set up Android SDK and tools**
3. **Enable USB debugging** on your Android device
4. **Connect device via USB**
5. **Verify device connection**:
   ```bash
   adb devices
   ```

## 📁 Project Structure

```
AndroidVisionApp/
├── src/                          # React Native source code
│   ├── App.tsx                   # Main app component
│   └── components/               # UI components
│       ├── ButtonRow.tsx         # Action buttons
│       ├── ImageViewer.tsx       # Image display
│       └── TextBox.tsx           # Results display
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
├── android/                      # Android-specific files
├── ios/                          # iOS-specific files (if needed)
├── package.json                  # Node.js dependencies
├── .gitignore                    # Git ignore rules
└── README.md                     # This file
```

## 💡 Code Logic Overview

### React Native Frontend Flow

1. **App Initialization** (`src/App.tsx`):
   - Performs health check with backend
   - Displays connection status
   - Handles camera permissions

2. **Image Capture**:
   - Uses `react-native-image-picker` for camera access
   - Captures high-quality photos
   - Converts to FormData for upload

3. **API Communication**:
   - Sends multipart/form-data to `/vision` endpoint
   - Displays loading indicator during processing
   - Shows extracted text results

### C# Backend Flow

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

### Frontend Development
```bash
# Start Metro bundler
npm start

# Build for Android (in separate terminal)
npx react-native run-android

# For debugging
npx react-native log-android
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

1. **Backend Connection Failed**:
   - Check if C# backend is running on port 5000
   - Verify IP address in React Native app
   - Ensure Windows Firewall allows port 5000
   - Confirm both devices on same WiFi network

2. **Python Integration Errors**:
   - Verify conda environment is active
   - Check Python path in `appsettings.json`
   - Ensure all Python packages are installed
   - Check Python scripts are in correct directory

3. **Android Build Issues**:
   - Clean and rebuild: `cd android && ./gradlew clean`
   - Check Android SDK installation
   - Verify USB debugging is enabled
   - Reset Metro bundler cache: `npx react-native start --reset-cache`

4. **AI Processing Failures**:
   - Verify GitHub token is valid and has permissions
   - Check network connectivity
   - Ensure image is valid format (JPEG/PNG)
   - Monitor backend logs for detailed error messages

### Debug Commands

```bash
# Check backend logs
cd csharp-backend && dotnet run

# Check Android device connection
adb devices

# View Android logs
npx react-native log-android

# Test backend health
curl http://localhost:5000/health

# Check Python environment
conda list
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

- ✅ **Camera Integration**: Native camera access with high-quality image capture
- ✅ **Vision AI Processing**: Advanced text extraction using GitHub AI models
- ✅ **Real-time Processing**: Live image processing with loading indicators
- ✅ **Cross-platform**: React Native app works on Android (iOS ready)
- ✅ **Scalable Backend**: C# .NET backend with Python AI integration
- ✅ **Error Handling**: Comprehensive error handling and user feedback
- ✅ **Logging**: Detailed logging for debugging and monitoring
- ✅ **Configuration**: Environment-based configuration for different deployments

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
