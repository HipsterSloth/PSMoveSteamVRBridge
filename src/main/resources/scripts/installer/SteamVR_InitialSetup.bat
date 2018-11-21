@echo off
setlocal

::Try to find the SteamVR runtime path from %LOCALAPPDATA%\openvr\openvrpaths.vrpath
SET FindSteamVRScriptPath=%~dp0FindSteamVR.ps1
FOR /f "delims=" %%a in ('powershell -NoProfile -ExecutionPolicy Bypass %FindSteamVRScriptPath%') DO SET "STEAMVR_RUNTIME_DIR=%%a"
IF %ERRORLEVEL% NEQ 0 (goto browse_for_steam_folder)
IF DEFINED STEAMVR_RUNTIME_DIR (goto write_set_drivers_script)

::Fall back to user specified selection of the Steam folder
:browse_for_steam_folder
set "psCommand="(new-object -COM 'Shell.Application').BrowseForFolder(0,'Please select the root folder for steam (ex: c:\Program Files (x86)\steam).',0,0).self.path""
for /f "usebackq delims=" %%I in (`powershell %psCommand%`) do set "STEAM_ROOT_PATH=%%I"
IF NOT DEFINED STEAM_ROOT_PATH (goto failure)

IF EXIST "%STEAM_ROOT_PATH%\steamapps\common\OpenVR" GOTO use_openvr
IF EXIST "%STEAM_ROOT_PATH%\steamapps\common\SteamVR" GOTO use_steamvr
goto no_steamvr_installed

:no_steamvr_installed
echo "No steamvr folder found at %STEAM_ROOT_PATH%! Please install steamvr."
goto failure

:use_openvr
set STEAMVR_RUNTIME_DIR=%STEAM_ROOT_PATH%\steamapps\common\OpenVR
goto write_set_drivers_script

:use_steamvr
set STEAMVR_RUNTIME_DIR=%STEAM_ROOT_PATH%\steamapps\common\SteamVR
goto write_set_drivers_script

:write_set_drivers_script
echo "Found SteamVR Runtime Dir: %STEAMVR_RUNTIME_DIR%"

pushd %~dp0..
set DRIVER_ABS_PATH=%CD%
popd

:: Write out the paths to a config batch file
del SteamVR_SetDriverVars.bat 2>NUL
echo @echo off >> SteamVR_SetDriverVars.bat
echo set INSTALL_DIR=%DRIVER_ABS_PATH%>> SteamVR_SetDriverVars.bat
echo set STEAMVR_RUNTIME_DIR=%STEAMVR_RUNTIME_DIR%>> SteamVR_SetDriverVars.bat

:: Copy over the openvr drivers
call SteamVR_ReregisterDrivers.bat
pause
goto exit

:failure
pause
goto exit

:exit
endlocal
