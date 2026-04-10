@echo off
setlocal
cls 

REM =================================================================================

echo. 
echo =====================================
echo Temporary PATH changes
echo =====================================
call :add_to_path "%PROGRAMFILES%\dotnet"
echo ... PATH updated for this session.
echo ... Close this window to clear them.

REM =================================================================================

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
dotnet build -c release
if errorlevel 1 (
    echo Build failed.
    goto end
)

REM =================================================================================

echo.
echo =====================================
echo Build completed successfully
echo =====================================
echo.
echo The executable should be located in:
echo.
echo    bin\Release\

REM =================================================================================
REM functions

goto end 

:add_to_path
set "P=%~1"
REM Normalize: remove trailing backslash (optional but safer)
if "%P:~-1%"=="\" set "P=%P:~0,-1%"
REM echo %PATH% | find /I ";%P%;" >nul
echo ;%PATH%; | find /I ";%P%;" >nul
if errorlevel 1 (
    set "PATH=%PATH%;%P%"
    echo Added: %P%
) else (
    echo Already in PATH: %P%
)
exit /b

REM =================================================================================

:end
pause 













REM -- END 

