@echo off
call SteamVR_SetDriverVars.bat

set "DEPRECATED_INSTALL_DIR=%STEAMVR_RUNTIME_DIR%drivers\psmove"
IF EXIST %DEPRECATED_INSTALL_DIR% (
  echo "Unregistering deprecated PSMove driver in SteamVR folder..."
  "%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" removedriver "%DEPRECATED_INSTALL_DIR%"
  
  echo "Deleting deprecated PSMove driver in SteamVR folder..."
  rmdir /s /q "%DEPRECATED_INSTALL_DIR%"
)

echo "Unregistering PSMove SteamVR driver..."
"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" removedriver "%INSTALL_DIR%"

if not defined suppressPause (
  echo "Done"
  pause
)