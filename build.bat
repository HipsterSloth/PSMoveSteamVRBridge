@echo off
setlocal

::Initialise
set PROJECT_ROOT=%cd%
set BUILD_PROPS_FILE=%PROJECT_ROOT%\build.properties

::Load configuration properties
call :loadBuildProperties || goto handleError

::Generate the project files for PSMoveService
call :generateProjectFiles || goto handleError

::Exit batch script
echo "BUILD SUCCESSFUL"
goto exit


::Function loads properties into the SetBuildVars batch file
:loadBuildProperties
echo Loading properties from %BUILD_PROPS_FILE%
call :loadBuildProperty "psmoveservice.package.url"  %BUILD_PROPS_FILE% PSM_PACKAGE_URL
call :loadBuildProperty "openvr.package.url" %BUILD_PROPS_FILE% OPENVR_PACKAGE_URL
call :loadBuildProperty "driver.version" %BUILD_PROPS_FILE% DRIVER_VERSION
call :loadBuildProperty "cmake.build.parameters" %BUILD_PROPS_FILE% BUILD_PARAMS
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
echo set OPENVR_PACKAGE_URL="%OPENVR_PACKAGE_URL%">> SetBuildVars.bat
echo set DRIVER_VERSION="%DRIVER_VERSION%">> SetBuildVars.bat
echo set BUILD_PARAMS="%BUILD_PARAMS%">> SetBuildVars.bat
goto:eof

::Function generates project files for the configured ide
:generateProjectFiles
@echo off
IF NOT EXIST %PROJECT_ROOT%\vs_project mkdir %PROJECT_ROOT%\ide
pushd %PROJECT_ROOT%\ide
echo "Rebuilding PSMoveSteamVRBridge Project files..."
echo "Running cmake in %PROJECT_ROOT%"
cmake .. -G "%BUILD_PARAMS%" -DDRIVER_VERSION="%DRIVER_VERSION%" -DPSM_PACKAGE_URL="%PSM_PACKAGE_URL%" -DOPENVR_PACKAGE_URL="%OPENVR_PACKAGE_URL%"
popd
goto:eof

:handleError
pause
endlocal
exit /b 1

:exit
endlocal
exit /b 0