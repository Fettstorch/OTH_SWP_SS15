@echo off
SETLOCAL EnableDelayedExpansion

call "%~dp0\SetEnv.bat" > NUL

REM ===========================================================================================
REM Script description
REM ===========================================================================================
set "-Script.Name=%~nx0"
set "-Script.Path=%~dp0"
set "-Script.Usage="^
Usage for !-Script.Name!: ^
Run ReportGenerator for OpenCover report.^
""

set "!-Script.Name!.Errorlevel=0"

REM configure ParseArguments
set "OptionDefaults=-reports:"""


REM ===========================================================================================
REM configure ParseOptions.bat GUI interface
REM ===========================================================================================
if not defined -reports.Usage              set "-reports.Usage="Select unit test report *_OpenCover.xml.""
if not defined -reports.Necessity          set "-reports.Necessity="Required""
if not defined -reports.Multiplicity       set "-reports.Multiplicity="Single""
if not defined -reports.GuiEntryType       set "-reports.GuiEntryType="File""
if not defined -reports.Values             set "-reports.Values="""
if not defined -reports.GuiDefaultValues   set "-reports.GuiDefaultValues="%SWP_BUILD_ROOT%\UnitTest\"

call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

rem output selected options
echo !-Script.Name!
for %%A in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%B in ("%%A") do (
  set name=!%%B!
  if /I "!%%B!" NEQ "" echo %%B=!name!
)
echo.

REM get report directory name
FOR /f %%i IN ("!-reports!") DO set "reportDir=%%~di%%~pi"

REM get plain target name
for %%F in ("!-reports!") do set "reportTarget=%%~nF"
set "reportTarget=!reportTarget:%SWP_LOCALTIME_DATESTAMP%_=!"
set reportTarget=!reportTarget:_OpenCover=!
echo !reportTarget!

set reportDir=!reportDir!\%SWP_LOCALTIME_DATESTAMP%_!reportTarget!_Report

echo Call: "%SWP_REPORTGENERATOR_CMD_EXE%" -reports:"!-reports!" -targetdir:"!reportDir!"
call "%SWP_REPORTGENERATOR_CMD_EXE%" -reports:"!-reports!" -targetdir:"!reportDir!"

IF DEFINED CALLED_FROM_EXPLORER pause
exit /b !%-Script.Name%.Errorlevel!
 No newline at end of file