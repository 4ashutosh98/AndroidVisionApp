@echo off
echo ===========================================
echo   Vision AI Web App - Quick Setup
echo ===========================================
echo.

echo [1/4] Installing dependencies...
cd web-frontend
call npm install

echo.
echo [2/4] Starting C# backend...
cd ..\csharp-backend
start "Backend Server" cmd /k "dotnet run"

echo.
echo [3/4] Waiting for backend to start...
timeout /t 5 /nobreak > nul

echo.
echo [4/4] Starting web frontend...
cd ..\web-frontend
echo.
echo ===========================================
echo   Setup Complete! 
echo   
echo   Backend: http://localhost:5000
echo   Frontend: http://localhost:3000
echo   
echo   The web app will open automatically.
echo ===========================================
echo.
start "Web Frontend" cmd /k "npm start"

echo.
echo Both servers are starting...
echo Press any key to exit this setup script.
pause > nul
