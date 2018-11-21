@echo off
call SteamVR_SetDriverVars.bat

echo "(Un)Registering PSMove SteamVR driver..."
"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" removedriver "%INSTALL_DIR%"

echo "Done"
pause