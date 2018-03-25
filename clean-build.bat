@echo off
setlocal

::Initialise
set PROJECT_ROOT=%cd%
set BUILD_PROPS_FILE=%PROJECT_ROOT%\build.properties

::Clean
call :cleanProjectFolder || goto handleError
call :cleanBuildFolder || goto handleError
call :cleanDependencies || goto handleError
echo "CLEAN SUCCESSFUL"

::Generate the project files for PSMoveService
call build.bat || goto handleError

::Exit batch script
goto exit


::---------------------------
::|Function definitions below
::---------------------------

::Clean up the old PSMoveSteamVRBridge ide project folder
:cleanProjectFolder
IF EXIST %PROJECT_ROOT%\ide (
echo "Cleaning old vs_project folder..."
del /f /s /q %PROJECT_ROOT%\ide > nul
rmdir /s /q %PROJECT_ROOT%\ide
)
goto:eof

::Clean up the old PSMoveSteamVRBridge build project folder
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

:handleError
pause
endlocal
exit /b 1

:exit
endlocal
exit /b 0