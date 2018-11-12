@echo off

::Initialise
set PROJECT_ROOT=%cd%
set BUILD_PROPS_FILE=%PROJECT_ROOT%\build.properties

::Load configuration properties
call :loadBuildProperties || goto handleError

::Generate the project files for PSMoveService
call :generateProjectFiles || goto handleError

::Exit batch script
goto exit


::Function loads build properties into local variables
:loadBuildProperties
echo Loading properties from %BUILD_PROPS_FILE%
call :loadBuildProperty "psmoveservice.package.url"  %BUILD_PROPS_FILE% PSM_PACKAGE_URL
call :loadBuildProperty "openvr.package.url" %BUILD_PROPS_FILE% OPENVR_PACKAGE_URL
call :loadBuildProperty "driver.version" %BUILD_PROPS_FILE% DRIVER_VERSION
call :loadBuildProperty "cmake.build.generator.windows" %BUILD_PROPS_FILE% BUILD_GENERATOR
call :loadBuildProperty "build.type" %BUILD_PROPS_FILE% BUILD_TYPE
echo Properties loaded successfully
goto:eof

::Fuction returns a configured build property value for the given key
:loadBuildProperty
set PROP_KEY=%1
set FILE=%2
echo "Loading %PROP_KEY% from %FILE%"
for /f "tokens=2,2 delims==" %%i in ('findstr /i %PROP_KEY% %FILE%') do set %3=%%i
goto:eof

::Function generates project files for the configured ide
:generateProjectFiles
@echo off
IF NOT EXIST %PROJECT_ROOT%\vs_project mkdir %PROJECT_ROOT%\generated
pushd %PROJECT_ROOT%\generated
echo "Rebuilding PSMoveSteamVRBridge Project files..."
echo "Running cmake in %PROJECT_ROOT%"
cmake .. -G "%BUILD_GENERATOR%" -DDRIVER_VERSION="%DRIVER_VERSION%" -DPSM_PACKAGE_URL="%PSM_PACKAGE_URL%" -DOPENVR_PACKAGE_URL="%OPENVR_PACKAGE_URL%" -DBUILD_TYPE="%BUILD_TYPE%"
popd
goto:eof

:handleError
echo "GENERATE FAILED"
exit /b 1
goto:eof

:exit
echo "GENERATE SUCCESSFUL"
exit /b 0
goto:eof
