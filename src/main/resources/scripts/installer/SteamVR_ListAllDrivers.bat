@echo off
call SteamVR_SetDriverVars.bat

echo "Listing registered SteamVR drivers..."
"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" show

echo "Done"
pause