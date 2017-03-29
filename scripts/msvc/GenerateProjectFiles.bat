@echo off
call SetBuildVars.bat

set PROJECT_ROOT=..\..

IF NOT EXIST %PROJECT_ROOT%\build mkdir %PROJECT_ROOT%\build
pushd %PROJECT_ROOT%\build

echo "Rebuilding PSMoveSteamVRBridge Project files..."
cmake .. -G %BUILD_CONFIGURATION% -DPSMOVESERVICE_BUILD_URL="%PSM_ZIP_FILE_PATH%"
pause

popd