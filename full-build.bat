@echo off
setlocal

::Initialise
set PROJECT_ROOT=%cd%
set BUILD_PROPS_FILE=%PROJECT_ROOT%\build.properties

::Clean
call clean.bat || goto handleError

::Generate the project files for PSMoveService
call generate.bat || goto handleError

::Build driver
call build.bat || goto handleError

::Exit batch script
goto exit


::---------------------------
::|Function definitions below
::---------------------------

:handleError
endlocal
exit /b 1
goto:eof

:exit
endlocal
exit /b 0
goto:eof