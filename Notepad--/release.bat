@echo off
setlocal

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
echo Building the Project (Release)
echo =====================================
dotnet build -c Release
if errorlevel 1 (
    echo Build failed.
    goto end
)

echo.
echo =====================================
echo Build completed successfully
echo =====================================
echo.
echo The executable should be located in:
echo.
echo    bin\Release\

:end
echo.
pause
