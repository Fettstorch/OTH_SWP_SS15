@echo off
REM ===========================================================================================
REM Assume explorer call if no arguments are given
REM ===========================================================================================
IF "%~1" == "" (
  set ENABLE_PARSE_OPTIONS_GUI_WIZARD=True
)

if not defined PARSE_OPTIONS_WAS_CALLED_ONCE (
  if defined ENABLE_PARSE_OPTIONS_GUI_WIZARD (
    call "%~dp0\CheckCallType.bat"
  )
)

call "%~dp0\ParseOptions.bat" %*
IF ERRORLEVEL 1 (
  echo Error - ParseOptions.bat
  set INTERNAL_ERROR_LVL=%ERRORLEVEL%
  goto :HANDLE_ERROR
)

REM ===========================================================================================
REM Overwrite default GUI values
REM ===========================================================================================
setlocal EnableDelayedExpansion
:: Validate and store the options, one at a time, using a loop.
:loop
  :: leave loop when there are no more parameters to handle
  set name=%~1
  if not defined name goto :break
  shift /1
  if "!OptionDefaults:%name%: =!" == "!OptionDefaults!" (
    rem The option provided expects a value. Set the default GUI value using the option as the name.
    rem and the next arg as the value
    endlocal & set "%name%.GuiDefaultValues=%~1"
    setlocal EnableDelayedExpansion
    shift /1
  ) 
goto :loop
:break
endlocal


IF NOT DEFINED PARSE_OPTIONS_WAS_CALLED_ONCE (
  IF DEFINED CALLED_FROM_EXPLORER (
    IF DEFINED ENABLE_PARSE_OPTIONS_GUI_WIZARD (
      call :OPEN_WIZARD
    )
  )
) 

IF DEFINED CALLED_FROM_EXPLORER (
  SET PARSE_OPTIONS_WAS_CALLED_ONCE=TRUE
  endlocal & set CALLED_FROM_EXPLORER=True
)

goto :EOF

:OPEN_WIZARD
echo Open wizard...
rem parse options via GUI
call "%~dp0\..\bin\QParseArgumentsWizard.exe"
IF NOT EXIST "envs.tmp" (
  echo Error while wizard configuration...
  exit /b 1
)

rem parse environment variables from envs.tmp and delete it afterwards
for /f "tokens=*" %%a in (envs.tmp) do (
  SET "%%a"
)
del "envs.tmp"
exit /b 0

:HANDLE_ERROR
echo Error - ParseOptionsGUI.bat
IF DEFINED CALLED_FROM_EXPLORER pause
exit /b !INTERNAL_ERROR_LVL!

:EOF
exit /b 0
