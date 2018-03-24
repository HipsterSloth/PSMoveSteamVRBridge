@echo off
setlocal

set PROJECT_ROOT=..\..

::Clean up the old PSMoveSteamVRBridge build folder
IF EXIST %PROJECT_ROOT%\build (
echo "Cleaning old build folder..."
del /f /s /q %PROJECT_ROOT%\build > nul
rmdir /s /q %PROJECT_ROOT%\build
)

::Clean up the old PSMoveSteamVRBridge deps folder
IF EXIST %PROJECT_ROOT%\deps (
echo "Cleaning old deps folder..."
del /f /s /q %PROJECT_ROOT%\deps > nul
rmdir /s /q %PROJECT_ROOT%\deps
)

::Select the path to the root Boost folder
set "psCommand="(new-object -COM 'Shell.Application')^
.BrowseForFolder(0,'Please select folder containing PSMoveService build zip.',0,0).self.path""
for /f "usebackq delims=" %%I in (`powershell %psCommand%`) do set "PSM_ZIP_ROOT_PATH=%%I"
if NOT DEFINED PSM_ZIP_ROOT_PATH (goto failure)

::Find the first build zip in the path given
for /R "%PSM_ZIP_ROOT_PATH%" %%f in (*.zip) do (
    if NOT DEFINED PSM_ZIP_FILE_PATH (
        set "PSM_ZIP_FILE_PATH=%%f"
        echo "Found build zip: %%f"
    ) else (
        echo "WARNING: addional build zip found: %%f"
    )
)

if NOT DEFINED PSM_ZIP_FILE_PATH (
    echo "Failed to find PSMoveService build zip in folder: %PSM_ZIP_ROOT_PATH%"
    goto failure
)

:: Change back slashes to forward slashes since cmake gets upset at backslashes in URL path
set PSM_ZIP_FILE_PATH=%PSM_ZIP_FILE_PATH:\=/%

:: Write out build parameters config batch file
del SetBuildVars.bat
echo @echo off >> SetBuildVars.bat
echo set BUILD_CONFIGURATION="Visual Studio 15 2017 Win64" >> SetBuildVars.bat
echo set PSM_ZIP_FILE_PATH=%PSM_ZIP_FILE_PATH% >> SetBuildVars.bat

:: Generate the project files for PSMoveService
call GenerateProjectFiles.bat
pause
goto exit

:failure
pause
goto exit

:exit
endlocal
