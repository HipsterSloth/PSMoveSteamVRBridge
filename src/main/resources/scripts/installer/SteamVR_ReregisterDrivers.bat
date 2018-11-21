@echo off
call SteamVR_SetDriverVars.bat

echo "(Re)Registering PSMove SteamVR driver..."
"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" adddriver "%INSTALL_DIR%"

echo "Done"
pause