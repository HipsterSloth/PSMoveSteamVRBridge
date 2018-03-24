@echo off
setlocal

::Initialise
set PROJECT_ROOT=..\..\..\..\..
set BUILD_PROPS_FILE=%PROJECT_ROOT%\src\main\resources\configuration\build.properties

::Clean
call :cleanBuildFolder || goto handleError
call :cleanDependencies || goto handleError

::Load configuration properties
call :loadBuildProperties || goto handleError

::Set build variables
call :setBuildVars || goto handleError

::Generate the project files for PSMoveService
REM call GenerateProjectFiles.bat || goto handleError

::Exit batch script
echo "MSVC solution generated successfully"
goto exit


::---------------------------
::|Function definitions below
::---------------------------

::Clean up the old PSMoveSteamVRBridge build folder
:cleanBuildFolder
IF EXIST %PROJECT_ROOT%\build (
echo "Cleaning old build folder..."
del /f /s /q %PROJECT_ROOT%\build > nul
rmdir /s /q %PROJECT_ROOT%\build
)
goto:eof

::Clean up the old PSMoveSteamVRBridge deps folder
:cleanDependencies
IF EXIST %PROJECT_ROOT%\deps (
echo "Cleaning old deps folder..."
del /f /s /q %PROJECT_ROOT%\deps > nul
rmdir /s /q %PROJECT_ROOT%\deps
)
goto:eof


::Function loads properties into the SetBuildVars batch file
:loadBuildProperties
echo Loading properties from %BUILD_PROPS_FILE%
call :loadBuildProperty "psmoveservice.pacakge.url"  %BUILD_PROPS_FILE% PSM_PACKAGE_URL
call :loadBuildProperty "openvr.pacakge.url" %BUILD_PROPS_FILE% OPENVR_PACAKGE_URL
call :loadBuildProperty "psmovesteamvrbridge.version" %BUILD_PROPS_FILE% PSMOVESTEAMVRBRIDGE_VERSION
call :loadBuildProperty "msbuild.parameters" %BUILD_PROPS_FILE% MSBUILD_PARAMS
echo Properties loaded into SetBuildVars.bat
goto:eof

::Fuction returns a configured build property value for the given key
:loadBuildProperty
set PROP_KEY=%1
set FILE=%2
echo "Loading %PROP_KEY% from %FILE%"
for /f "tokens=2,2 delims==" %%i in ('findstr /i %PROP_KEY% %FILE%') do set %3=%%i
goto:eof

::Function sets the build variables in a batch file called SetBuildVars.bat
:setBuildVars
del SetBuildVars.bat
echo @echo off >> SetBuildVars.bat
echo set PSM_PACKAGE_URL="%PSM_PACKAGE_URL%">> SetBuildVars.bat
echo set OPENVR_PACAKGE_URL="%OPENVR_PACAKGE_URL%">> SetBuildVars.bat
echo set PSMOVESTEAMVRBRIDGE_VERSION="%PSMOVESTEAMVRBRIDGE_VERSION%">> SetBuildVars.bat
echo set MSBUILD_PARAMS="%MSBUILD_PARAMS%">> SetBuildVars.bat
goto:eof

:handleError
pause
endlocal
exit /b 1

:exit
endlocal
exit /b 0