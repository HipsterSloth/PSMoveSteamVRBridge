@echo off
setlocal

::Initialise
set PROJECT_ROOT=..\..\..\..
set BUILD_PROPS_FILE=%PROJECT_ROOT%\src\main\resources\configuration\build.properties

::Clean
call :cleanBuildFolder || goto handleError
call :cleanDependencies || goto handleError

::Load configuration properties
call :loadBuildProperties || goto handleError

::Set build variables
call :setBuildVars || goto handleError

::Generate the project files for PSMoveService
call GenerateProjectFiles.bat || goto handleError

::Exit batch script
echo "BUILD SUCCESSFUL"
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
IF EXIST %PROJECT_ROOT%\libs (
echo "Cleaning old deps folder..."
del /f /s /q %PROJECT_ROOT%\libs > nul
rmdir /s /q %PROJECT_ROOT%\libs
)
goto:eof

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

:handleError
pause
endlocal
exit /b 1

:exit
endlocal
exit /b 0