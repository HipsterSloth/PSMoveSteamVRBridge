@echo off


::Build driver
call :buildDriver || goto handleError

::Exit batch script
goto exit

::---------------------------
::|Function definitions below
::---------------------------

::Function runs INSTALL cmake target which will build the driver as either debug/release
:buildDriver
echo "Build Type=%BUILD_TYPE%"
cmake --build generated --target INSTALL --config %BUILD_TYPE%
goto:eof

:handleError
echo "BUILD FAILED"
endlocal
exit /b 1
goto:eof

:exit
echo "BUILD SUCCESSFUL"
endlocal
exit /b 0
goto:eof
