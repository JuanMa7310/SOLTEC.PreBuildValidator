@echo off
setlocal
cd /d %~dp0

echo Running SOLTEC.PreBuildValidator...

dotnet build SOLTEC.PreBuildValidator.csproj
if %ERRORLEVEL% NEQ 0 (
    echo Build failed.
    exit /b %ERRORLEVEL%
)

dotnet bin\Debug\net8.0\SOLTEC.PreBuildValidator.dll
if %ERRORLEVEL% NEQ 0 (
    echo Validator failed.
    exit /b %ERRORLEVEL%
)

echo Validator completed successfully.
pause
endlocal
