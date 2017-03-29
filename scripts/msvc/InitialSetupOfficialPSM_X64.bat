@echo off
setlocal

set PROJECT_ROOT=..\..

::Clean up the old PSMoveSteamVRBridge build folder
IF EXIST %PROJECT_ROOT%\build (
echo "Cleaning old build folder..."
del /f /s /q %PROJECT_ROOT%\build > nul
rmdir /s /q %PROJECT_ROOT%\build
)

::Clean up the old PSMoveSteamVRBridge deps folder
IF EXIST %PROJECT_ROOT%\deps (
echo "Cleaning old deps folder..."
del /f /s /q %PROJECT_ROOT%\deps > nul
rmdir /s /q %PROJECT_ROOT%\deps
)

::Get the official PSMoveService build URL
set /p PSM_OFFICIAL_BUILD_URL=<%PROJECT_ROOT%\PSMoveServiceReleaseURL.txt
echo "Setting build URL to %PSM_OFFICIAL_BUILD_URL%"

:: Write out build parameters config batch file
del SetBuildVars.bat
echo @echo off >> SetBuildVars.bat
echo set BUILD_CONFIGURATION="Visual Studio 14 2015 Win64" >> SetBuildVars.bat
echo set PSM_ZIP_FILE_PATH=%PSM_OFFICIAL_BUILD_URL% >> SetBuildVars.bat

:: Generate the project files for PSMoveService
call GenerateProjectFiles.bat
pause
goto exit

:failure
pause
goto exit

:exit
endlocal
