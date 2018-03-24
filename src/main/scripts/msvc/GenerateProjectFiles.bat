@echo off
call SetBuildVars.bat

set PROJECT_ROOT=..\..\..\..

IF NOT EXIST %PROJECT_ROOT%\build mkdir %PROJECT_ROOT%\build
pushd %PROJECT_ROOT%\build

echo "Rebuilding PSMoveSteamVRBridge Project files..."
echo "Running cmake in %PROJECT_ROOT%"
cmake .. -G %BUILD_PARAMS% -DDRIVER_VERSION="%DRIVER_VERSION%" -DPSM_PACKAGE_URL="%PSM_PACKAGE_URL%" -DOPENVR_PACKAGE_URL="%OPENVR_PACKAGE_URL%"
popd