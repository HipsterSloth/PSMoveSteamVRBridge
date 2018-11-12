@echo off
call SteamVR_SetDriverVarsWin64.bat

echo "(Re)Installing PSMoveService SteamVR Win64 driver..."

cd ../

IF NOT EXIST "%INSTALL_DIR%\bin\win64" mkdir "%INSTALL_DIR%\bin\win64"

xcopy /y *.dll "%INSTALL_DIR%\bin\win64"

IF EXIST *.pdb (
	xcopy /y *.pdb "%INSTALL_DIR%\bin\win64"
	IF EXIST "%SystemDrive%\Windows\system32\ucrtbase.dll" ( copy "%SystemDrive%\Windows\system32\ucrtbase.dll" "%INSTALL_DIR%\bin\win64" ) ELSE (echo "Couldn't find the required C Runtime Win32 UCRTBASE.DLL libraries")
	IF EXIST "%SystemDrive%\Windows\system32\ucrtbased.dll" ( copy "%SystemDrive%\Windows\system32\ucrtbased.dll" "%INSTALL_DIR%\bin\win64" ) ELSE (echo "Couldn't find the required C Runtime Win32 UCRTBASED.DLL libraries")
)
		

copy monitor_psmove.exe "%INSTALL_DIR%\bin\win64"
xcopy /s /i /y "resources" "%STEAMVR_RUNTIME_DIR%\drivers\psmove\resources"
xcopy /s /i /y configuration "%STEAMVR_RUNTIME_DIR%\drivers\psmove\configuration"
copy driver.vrdrivermanifest "%INSTALL_DIR%"

"%STEAMVR_RUNTIME_DIR%\bin\win64\vrpathreg" adddriver "%INSTALL_DIR%"

echo "Done"
pause