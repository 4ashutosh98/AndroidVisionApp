# Quick Deployment Guide

## 🚀 Get Your Vision AI App Live in Minutes!

### Option 1: Local Network (Immediate)
```bash
# 1. Start backend
cd csharp-backend
dotnet run --urls="http://0.0.0.0:5000"

# 2. Update frontend config
# Edit web-frontend/src/App.tsx
const BACKEND_URL = 'http://YOUR_PC_IP:5000';

# 3. Start frontend
cd web-frontend
npm start

# 4. Share the link
# Access from any device: http://YOUR_PC_IP:3000
```

### Option 2: Cloud Deployment (Production)

#### Backend (Azure/AWS/DigitalOcean)
```bash
# Build for production
cd csharp-backend
dotnet publish -c Release -o ./publish

# Deploy to cloud service
# Update appsettings.json with production URLs
```

#### Frontend (Netlify/Vercel - FREE)
```bash
# Build for production
cd web-frontend
npm run build

# Deploy to Netlify (drag & drop the 'build' folder)
# OR deploy to Vercel with CLI:
npx vercel --prod
```

### Option 3: Docker (All Platforms)
```dockerfile
# See Dockerfile examples in project root
docker-compose up -d
```

## 🌐 Custom Domain
- Point your domain to the deployed frontend
- Update CORS settings in backend
- Enable HTTPS for camera access

Ready to go live? Choose your deployment method and launch! 🚀
