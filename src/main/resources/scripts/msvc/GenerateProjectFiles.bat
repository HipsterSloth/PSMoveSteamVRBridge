@echo off
call SetBuildVars.bat

set PROJECT_ROOT=..\..\..\..\..

IF NOT EXIST %PROJECT_ROOT%\build mkdir %PROJECT_ROOT%\build
pushd %PROJECT_ROOT%\build

echo "Rebuilding PSMoveSteamVRBridge Project files..."
echo "Running cmake in %PROJECT_ROOT%
cmake .. -G %MSBUILD_PARAMS%^
 -DPSMOVESERVICE_PACKAGE_URL="%PSMOVESERVICE_BUILD_URL%"^
 -DOPENVR_PACKAGE_URL="%OPENVR_PACKAGE_URL%"
 -DPSMOVESTEAMVRBRIDGE_VERSION="%PSMOVESTEAMVRBRIDGE_VERSION%"^
pause

popd