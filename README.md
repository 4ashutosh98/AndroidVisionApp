# Android Vision App - React Native Vision LLM

A React Native Android application that provides AI-powered text extraction from images using Vision LLM technology via Azure AI Inference SDK (GitHub AI for development, Azure AI for production).

## Architecture Overview

- **Frontend**: React Native app with camera integration and image capture
- **Backend**: Node.js/Express server with image processing and AI inference
- **AI Service**: Multimodal LLM via Azure AI Inference SDK (GitHub AI for development, Azure AI for production)
- **Image Storage**: Temporary local storage with automatic cleanup

## Strategic Architecture Decisions

### Why Azure AI Inference SDK?
This project uses the **Azure AI Inference SDK** for all AI operations, even when connecting to GitHub AI models. This strategic choice provides:

1. **Seamless Migration Path**: Switch from GitHub AI to Azure AI with minimal code changes
2. **Consistent API Interface**: Same SDK methods regardless of the underlying AI provider
3. **Enterprise Readiness**: Built for production Azure environments
4. **Future-Proofing**: Easy integration with Azure's expanding AI service portfolio

### Development vs Production Setup
- **Development**: GitHub AI models (rate-limited but free with GitHub Pro)
- **Production**: Azure AI services (enterprise-scale, SLA-backed, fully managed)

The backend code is designed to switch between providers by simply changing the endpoint URL and authentication credentials.

## Prerequisites

### Required Software
- **Node.js** (v18 or higher)
- **React Native CLI** (`npm install -g @react-native-community/cli`)
- **Android Studio** with Android SDK
- **Java Development Kit (JDK 17)**
- **Git**

### Required Accounts
- **GitHub Account** with GitHub Pro (for development with GitHub AI models)
- **Azure Subscription** (for production with Azure AI services)
- **GitHub Personal Access Token** (development setup)
- **Azure AI Service Keys** (production setup)

### Android Development Setup
Complete the [React Native Environment Setup](https://reactnative.dev/docs/set-up-your-environment) for Android development.

## Setup Instructions

### 1. Clone and Install Dependencies

```bash
# Clone the repository
git clone <your-repo-url>
cd AndroidVisionApp

# Install frontend dependencies
npm install

# Install backend dependencies
cd backend
npm install
cd ..
```

## AI Service Configuration

### Option A: GitHub AI Setup (Development)

This is the current development configuration using GitHub AI models as a cost-effective development solution with GitHub Pro.

#### Step A.1: Get GitHub Personal Access Token
1. Go to [GitHub Settings > Developer Settings > Personal Access Tokens](https://github.com/settings/tokens)
2. Click "Generate new token (classic)"
3. Select scopes:
   - `repo` (Full control of private repositories)
   - `read:packages` (Download packages from GitHub Package Registry)
4. Copy the generated token - **you won't see it again!**

#### Step A.2: Create Backend Environment File (GitHub AI)
Create a `.env` file in the `backend/` directory:

```bash
cd backend
touch .env
```

Add your GitHub token to the `.env` file:
```env
# GitHub AI API Configuration (Development)
GITHUB_TOKEN=your_github_personal_access_token_here
PORT=3000
AI_PROVIDER=github
```

### Option B: Azure AI Setup (Production)

For production deployment, switch to Azure AI services for enterprise-grade performance, reliability, and SLA.

#### Step B.1: Create Azure AI Service
1. **Log into Azure Portal**: https://portal.azure.com
2. **Create AI Service**:
   - Click "Create a resource"
   - Search for "Azure AI services" or "Cognitive Services"
   - Select "Azure AI services multi-service account"
3. **Configure the Service**:
   - **Subscription**: Choose your Azure subscription
   - **Resource Group**: Create new or select existing
   - **Region**: Choose region closest to your users (e.g., East US, West Europe)
   - **Name**: Choose a unique name (e.g., `your-company-ai-service`)
   - **Pricing Tier**: Select appropriate tier (S0 for production, F0 for testing)
4. **Click "Review + Create"** then **"Create"**

#### Step B.2: Get Azure AI Service Keys
1. **Navigate to your AI service** in the Azure Portal
2. **Go to "Keys and Endpoint"** in the left sidebar
3. **Copy the following**:
   - **Key 1** (or Key 2)
   - **Endpoint URL** (e.g., `https://your-service.cognitiveservices.azure.com/`)
   - **Region** (e.g., `eastus`)

#### Step B.3: Create Backend Environment File (Azure AI)
Create a `.env` file in the `backend/` directory:

```bash
cd backend
touch .env
```

Add your Azure AI credentials to the `.env` file:
```env
# Azure AI API Configuration (Production)
AZURE_AI_KEY=your_azure_ai_service_key_here
AZURE_AI_ENDPOINT=https://your-service.cognitiveservices.azure.com/
AZURE_AI_REGION=eastus
PORT=3000
AI_PROVIDER=azure
```

#### Step B.4: Deploy AI Models (Azure)

For Azure AI, you'll need to deploy the vision model:

1. **Go to Azure AI Studio**: https://ai.azure.com/
2. **Navigate to Model Catalog**
3. **Find Vision Models**:
   - Search for "GPT-4 Vision" or "GPT-4o" for multimodal capabilities
   - Or "Llama-3.2-Vision" if available in your region
4. **Deploy the Model**:
   - Click "Deploy"
   - Choose deployment name (e.g., `vision-model-deployment`)
   - Select compute resources
   - Configure scaling settings
5. **Note the Deployment Details**:
   - Deployment name
   - Model version
   - Endpoint URL

Update your `.env` file with deployment details:
```env
# Azure AI API Configuration (Production)
AZURE_AI_KEY=your_azure_ai_service_key_here
AZURE_AI_ENDPOINT=https://your-service.cognitiveservices.azure.com/
AZURE_AI_REGION=eastus
AZURE_AI_MODEL_DEPLOYMENT=vision-model-deployment
AZURE_AI_MODEL_NAME=gpt-4o
PORT=3000
AI_PROVIDER=azure
```

**Important Security Notes**: 
- Replace placeholder values with your actual credentials
- **Never commit the `.env` file to version control**
- The `.env` file is already in `.gitignore`
- Use Azure Key Vault for production credential management
- Rotate API keys regularly for security

### 3. Backend AI Client Configuration

### 3. Backend AI Client Configuration

The backend is architected to work with both GitHub AI and Azure AI using the same Azure AI Inference SDK. This provides a seamless migration path from development to production.

#### Current Implementation (GitHub AI)
```javascript
// GitHub AI multimodal inference (Development)
const ghEndpoint = 'https://models.github.ai/inference';
const ghToken = process.env.GITHUB_TOKEN;
const ghModel = 'meta/Llama-3.2-11B-Vision-Instruct';
const ghClient = ModelClient(ghEndpoint, new AzureKeyCredential(ghToken));
```

#### Azure AI Implementation (Production Ready)
To switch to Azure AI, update `backend/server.js`:

```javascript
// Azure AI multimodal inference (Production)
const azureEndpoint = process.env.AZURE_AI_ENDPOINT;
const azureKey = process.env.AZURE_AI_KEY;
const azureModel = process.env.AZURE_AI_MODEL_NAME || 'gpt-4o';
const azureClient = ModelClient(azureEndpoint, new AzureKeyCredential(azureKey));
```

#### Universal Configuration (Recommended)
For a production-ready setup that supports both providers:

```javascript
// Universal AI client configuration
const AI_PROVIDER = process.env.AI_PROVIDER || 'github';

let aiClient, aiModel, aiEndpoint;

if (AI_PROVIDER === 'azure') {
  aiEndpoint = process.env.AZURE_AI_ENDPOINT;
  aiClient = ModelClient(aiEndpoint, new AzureKeyCredential(process.env.AZURE_AI_KEY));
  aiModel = process.env.AZURE_AI_MODEL_NAME || 'gpt-4o';
  console.log('[Backend] Using Azure AI service');
} else {
  aiEndpoint = 'https://models.github.ai/inference';
  aiClient = ModelClient(aiEndpoint, new AzureKeyCredential(process.env.GITHUB_TOKEN));
  aiModel = 'meta/Llama-3.2-11B-Vision-Instruct';
  console.log('[Backend] Using GitHub AI service');
}
```

#### Key Backend Features:
- **Automatic uploads folder creation** on server startup
- **Multer disk storage** for temporary image files
- **Static file serving** for image access during inference
- **Automatic file cleanup** after processing
- **CORS enabled** for React Native frontend
- **Health check endpoint** for connectivity testing

### 4. Frontend Configuration

#### Step 4.1: Update Backend URL
Edit `src/App.tsx` and update the `BACKEND_URL` with your development machine's IP:

```typescript
// Replace with your dev machine IP reachable by Android device
const BACKEND_URL = 'http://YOUR_IP_ADDRESS:3000';
```

To find your IP address:
- **Windows**: `ipconfig` (look for IPv4 Address)
- **Mac/Linux**: `ifconfig` or `ip addr show`

#### Step 4.2: Android Network Configuration
The app already includes network security configuration in `android/app/src/debug/AndroidManifest.xml`:

```xml
<application
  android:networkSecurityConfig="@xml/network_security_config"
  android:usesCleartextTraffic="true">
```

This allows HTTP connections to your development server.

### 5. Image Capture Configuration

The frontend is configured for **full-quality image capture** without compression:

```typescript
const options: CameraOptions = {
  mediaType: 'photo',
  saveToPhotos: false,
  // Removed all compression settings to send full-quality images
};
```

This ensures maximum accuracy for Vision LLM processing.

## Running the Application

### 1. Start the Backend Server

```bash
cd backend
npm start
```

You should see output based on your AI provider:

**GitHub AI:**
```
[Backend] Uploads directory ensured: /path/to/backend/uploads
[Backend] uploads directory: /path/to/backend/uploads
[Backend] Using GitHub AI service
GITHUB_TOKEN= true
Backend listening on port 3000
```

**Azure AI:**
```
[Backend] Uploads directory ensured: /path/to/backend/uploads
[Backend] uploads directory: /path/to/backend/uploads
[Backend] Using Azure AI service
AZURE_AI_KEY= true
Backend listening on port 3000
```

### 2. Start React Native Metro Server

In a new terminal:
```bash
# From the root directory
npm start
```

### 3. Run the Android App

In another terminal:
```bash
npm run android
```

Or build and run directly from Android Studio.

## API Endpoints

### Health Check
- **URL**: `GET /health`
- **Response**: `{"status": "ok"}`
- **Purpose**: Frontend connectivity testing

### Vision LLM Processing
- **URL**: `POST /vision`
- **Content-Type**: `multipart/form-data`
- **Body**: Image file as `image` field
- **Process**:
  1. Saves uploaded image to `uploads/` folder
  2. Creates public URL for image access
  3. Sends image to AI model (GitHub AI or Azure AI based on configuration)
  4. Returns extracted text from multimodal LLM
  5. Automatically deletes uploaded image
- **Response**: `{"result": "extracted text", "imageUrl": "public_url"}`

## Azure AI Inference SDK Usage

### Dependencies
```json
{
  "@azure-rest/ai-inference": "latest",
  "@azure/core-auth": "latest"
}
```

### Implementation
```javascript
import ModelClient, { isUnexpected } from '@azure-rest/ai-inference';
import { AzureKeyCredential } from '@azure/core-auth';

// Initialize client (works for both GitHub AI and Azure AI)
const aiClient = ModelClient(endpoint, new AzureKeyCredential(apiKey));

// Vision inference request (identical API for both providers)
const response = await aiClient.path('/chat/completions').post({
  body: {
    messages: [
      {
        role: 'system',
        content: 'You are a helpful assistant that can read and extract text from images.'
      },
      {
        role: 'user',
        content: [
          {
            type: 'text',
            text: 'Please extract all the text visible in this image.'
          },
          {
            type: 'image_url',
            image_url: {
              url: imageDataUrl // Base64 encoded image
            }
          }
        ]
      }
    ],
    model: modelName,
    temperature: 0.1,
    max_tokens: 1000
  }
});

// Handle response (identical for both providers)
if (isUnexpected(response)) {
  throw new Error(response.body?.error?.message || 'AI inference failed');
}

const extractedText = response.body.choices[0].message.content;
```

### Supported Models

#### GitHub AI Models (Development)
- **Llama-3.2-11B-Vision-Instruct**: Multimodal model for vision and text
- **Rate Limits**: GitHub Pro provides generous limits for development
- **Cost**: Free with GitHub Pro subscription

#### Azure AI Models (Production)
- **GPT-4o**: Latest multimodal model with vision capabilities
- **GPT-4 Vision**: Dedicated vision-language model
- **Llama models**: Open-source alternatives (if available in your region)
- **Custom Models**: Deploy your own fine-tuned models
- **Scaling**: Auto-scaling based on demand
- **SLA**: Enterprise-grade service level agreements

### Migration from GitHub AI to Azure AI

To migrate from development (GitHub AI) to production (Azure AI):

1. **Update Environment Variables**:
   ```env
   # Change from GitHub AI configuration
   # GITHUB_TOKEN=xxx
   # AI_PROVIDER=github
   
   # To Azure AI configuration
   AZURE_AI_KEY=your_azure_key
   AZURE_AI_ENDPOINT=https://your-service.cognitiveservices.azure.com/
   AZURE_AI_MODEL_NAME=gpt-4o
   AI_PROVIDER=azure
   ```

2. **Update Backend Code** (if using universal configuration):
   - No code changes needed! The same Azure AI Inference SDK handles both providers
   - Only environment variables need to be updated

3. **Test the Migration**:
   - Start backend with new environment variables
   - Verify AI provider logs show "Using Azure AI service"
   - Test vision inference functionality
   - Monitor response times and accuracy

### Benefits of This Architecture

#### Development Benefits
- **Cost-Effective**: Free development with GitHub Pro
- **Quick Setup**: Minimal configuration required
- **Rate Limits**: Sufficient for development and testing

#### Production Benefits
- **Enterprise Scale**: Azure AI handles production workloads
- **SLA Guarantees**: 99.9% uptime commitment
- **Global Deployment**: Multiple regions for low latency
- **Security**: Enterprise-grade security and compliance
- **Cost Control**: Pay-per-use with predictable pricing

#### Migration Benefits
- **Zero Code Changes**: Same SDK for both providers
- **Gradual Migration**: Switch services without rewriting application logic
- **Consistent API**: Identical request/response patterns
- **Future-Proof**: Easy to adopt new Azure AI features

## Frontend Features

### UI Components
- **Health Status Indicator**: Shows backend connectivity
- **Vision LLM Button**: Initiates AI-powered text extraction
- **Loading Indicator**: Shows during model inference
- **Error Display**: Shows API errors
- **Image Preview**: Displays captured image
- **Scrollable Text Box**: Shows extracted text results
- **Clear Function**: Resets image and text

### Image Processing Flow
1. User taps Vision LLM button
2. Camera launches with full-quality settings
3. User captures image
4. Image sent to backend vision endpoint
5. Backend processes with AI model
6. Results displayed in text box
7. Image automatically cleaned up on backend

## Troubleshooting

### Backend Issues

#### GitHub AI Issues
- **"GITHUB_TOKEN= false"**: GitHub token not found or invalid
  - Verify token is in `.env` file
  - Ensure token has correct scopes (`repo`, `read:packages`)
  - Check token hasn't expired
- **Rate Limiting**: GitHub AI has rate limits even with Pro
  - Wait for rate limit reset
  - Consider switching to Azure AI for higher limits
- **Model Not Available**: Some models may be region-restricted
  - Try different model names
  - Check GitHub AI model availability

#### Azure AI Issues
- **"AZURE_AI_KEY= false"**: Azure credentials not found or invalid
  - Verify all Azure environment variables are set
  - Check Azure AI service is properly created
  - Ensure API keys are correct and not expired
- **Model Deployment Issues**: Model not deployed or accessible
  - Verify model is deployed in Azure AI Studio
  - Check deployment name matches environment variable
  - Ensure sufficient quota and compute resources
- **Regional Availability**: Some models only available in specific regions
  - Check model availability in your Azure region
  - Consider deploying in different region if needed
- **Quota Exceeded**: Azure AI usage limits reached
  - Check quota usage in Azure Portal
  - Request quota increases if needed
  - Monitor usage patterns

#### Common Backend Issues
- **Port Already in Use**: Port 3000 occupied by another process
  - Kill existing process: `npx kill-port 3000`
  - Change port in `.env`: `PORT=3001`
- **Uploads Folder Issues**: Permission errors
  - Check folder permissions
  - Ensure backend has write access to project directory
- **CORS Errors**: Frontend can't connect to backend
  - Verify CORS is enabled in server.js
  - Check frontend URL matches backend configuration

### Frontend Issues

#### React Native Setup
- **Android Build Failures**: 
  - Clean build: `cd android && ./gradlew clean && cd ..`
  - Rebuild: `npm run android`
  - Check Android SDK and Java versions
- **Network Connectivity**: App can't reach backend
  - Verify backend URL in `src/App.tsx`
  - Use your machine's IP address, not `localhost`
  - Check network security configuration
- **Image Picker Issues**: Camera not working
  - Verify camera permissions in Android manifest
  - Test on physical device (camera doesn't work in emulator)
  - Check React Native image picker setup

#### Network Configuration
- **HTTP vs HTTPS**: Mixed content errors
  - Backend uses HTTP for development
  - Ensure Android app allows clear text traffic
  - For production, use HTTPS with proper SSL certificates

### API Response Issues

#### GitHub AI Responses
- **Authentication Errors**: Token invalid or insufficient permissions
- **Model Errors**: Specific model unavailable or overloaded
- **Rate Limits**: API calls exceeded for billing period

#### Azure AI Responses
- **Deployment Errors**: Model deployment not found or inactive
- **Quota Errors**: Usage limits exceeded for subscription
- **Regional Errors**: Model not available in selected region

### Performance Issues

#### Image Processing
- **Large Images**: Processing very large images may timeout
  - Consider implementing client-side image resizing
  - Monitor backend memory usage
- **Network Timeouts**: Slow internet connection
  - Increase timeout values in axios configuration
  - Implement retry logic for failed requests

#### AI Model Performance
- **Slow Responses**: Model inference taking too long
  - GitHub AI: Try during off-peak hours
  - Azure AI: Scale up compute resources or use faster models
  - Monitor response times and set appropriate timeouts

### Development Tips

#### Environment Management
- **Multiple Environments**: Use different `.env` files
  ```bash
  cp .env .env.development
  cp .env .env.production
  ```
- **Configuration Validation**: Add startup checks
  ```javascript
  // Validate required environment variables
  const requiredEnvVars = AI_PROVIDER === 'azure' 
    ? ['AZURE_AI_KEY', 'AZURE_AI_ENDPOINT'] 
    : ['GITHUB_TOKEN'];
  
  for (const envVar of requiredEnvVars) {
    if (!process.env[envVar]) {
      throw new Error(`Missing required environment variable: ${envVar}`);
    }
  }
  ```

#### Debugging
- **Enable Verbose Logging**: Add more console.log statements
- **Network Debugging**: Use tools like Postman to test API endpoints
- **Frontend Debugging**: Use React Native debugger for frontend issues

#### Testing Strategy
1. **Test Backend Independently**: Use curl or Postman
2. **Test Frontend Components**: Test UI components separately
3. **Integration Testing**: Test full image capture → processing → display flow
4. **Provider Switching**: Test both GitHub AI and Azure AI configurations

### Getting Help

#### Documentation
- [Azure AI Services Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/)
- [Azure AI Inference SDK](https://github.com/Azure/azure-sdk-for-js/tree/main/sdk/ai/ai-inference-rest)
- [GitHub AI Models](https://github.com/marketplace/models)
- [React Native Documentation](https://reactnative.dev/docs/getting-started)

#### Support Channels
- **Azure AI**: Azure Support Portal or Stack Overflow with `azure-cognitive-services` tag
- **GitHub AI**: GitHub Support or GitHub Community Discussions
- **React Native**: React Native Community Discord or Stack Overflow

#### Best Practices for Production
- **Monitoring**: Implement application monitoring (Azure Application Insights)
- **Error Handling**: Add comprehensive error handling and logging
- **Security**: Use Azure Key Vault for credential management
- **Scaling**: Plan for horizontal scaling with multiple backend instances
- **Caching**: Implement caching for frequently processed images
- **Cost Management**: Monitor AI service usage and costs
