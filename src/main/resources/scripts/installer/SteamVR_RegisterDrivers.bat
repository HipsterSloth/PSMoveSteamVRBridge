@echo off
call SteamVR_SetDriverVars.bat

echo "Registering PSMove SteamVR driver..."
"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" adddriver "%INSTALL_DIR%"

if not defined suppressPause (
  echo "Done"
  pause
)