@echo off
call SteamVR_SetDriverVarsWin64.bat

echo "Listing SteamVR Win64 driver..."
"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" show

echo "Done"
pause