@echo off
setlocal

cls 
echo.
echo =====================================
echo Restoring NuGet Packages
echo =====================================
dotnet restore
if errorlevel 1 (
    echo Failed to restore packages.
    goto end
)

echo.
echo =====================================
echo Building the Project (Debug)
echo =====================================
dotnet run -c Debug
if errorlevel 1 (
    echo Build failed.
    goto end
)

:end
echo.
pause
