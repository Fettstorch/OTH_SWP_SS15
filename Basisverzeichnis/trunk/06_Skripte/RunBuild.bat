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
Build.bat for building a solution via command line.^
""

set "!-Script.Name!.Errorlevel=0"

REM configure ParseArguments
set "OptionDefaults=-solution:"" -configuration:"" -logDir:"" -additionalOptions:"""

REM ===========================================================================================
REM configure ParseOptions.bat GUI interface
REM ===========================================================================================
if not defined -solution.Usage              set "-solution.Usage="Select C# solution that should be built.""
if not defined -solution.Necessity          set "-solution.Necessity="Required""
if not defined -solution.Multiplicity       set "-solution.Multiplicity="Single""
if not defined -solution.GuiEntryType       set "-solution.GuiEntryType="File""
if not defined -solution.Values             set "-solution.Values="""
if not defined -solution.GuiDefaultValues   set "-solution.GuiDefaultValues="%SWP_BRANCH_ROOT%\03_Implementierung\src\src.sln""

if not defined -configuration.Usage              set "-configuration.Usage="Select configuration that should to built.""
if not defined -configuration.Necessity          set "-configuration.Necessity="Required""
if not defined -configuration.Multiplicity       set "-configuration.Multiplicity="Single""
if not defined -configuration.GuiEntryType       set "-configuration.GuiEntryType="Default""
if not defined -configuration.Values             set "-configuration.Values="Debug^|Release""
if not defined -configuration.GuiDefaultValues   set "-configuration.GuiDefaultValues="Release""

if not defined -logDir.Usage              set "-logDir.Usage="Define root directory for build log file.""
if not defined -logDir.Necessity          set "-logDir.Necessity="Optional""
if not defined -logDir.Multiplicity       set "-logDir.Multiplicity="Single""
if not defined -logDir.GuiEntryType       set "-logDir.GuiEntryType="Folder""
if not defined -logDir.Values             set "-logDir.Values="""
if not defined -logDir.GuiDefaultValues   set "-logDir.GuiDefaultValues="""

if not defined -additionalOptions.Usage              set "-additionalOptions.Usage="Add additional options for devenv.com.""
if not defined -additionalOptions.Necessity          set "-additionalOptions.Necessity="Optional""
if not defined -additionalOptions.Multiplicity       set "-additionalOptions.Multiplicity="Single""
if not defined -additionalOptions.Values             set "-additionalOptions.Values="""
if not defined -additionalOptions.GuiDefaultValues   set "-additionalOptions.GuiDefaultValues="/rebuild""

call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

rem output selected options
echo !-Script.Name!
for %%A in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%B in ("%%A") do (
  set name=!%%B!
  if /I "!%%B!" NEQ "" echo %%B=!name!
)
echo.

REM get plain solution name
for %%F in ("!-solution!") do set "solutionName=%%~nF"

REM define log directory and log file
set logDir=!-logDir!
if not defined -logDir (
  set "logDir=%SWP_BRANCH_ROOT%\Build\!-configuration!\!solutionName!\Log"
  if not exist !logDir! (
    echo Create !logDir!.
    mkdir "%SWP_BRANCH_ROOT%\Build\!-configuration!\!solutionName!\Log"
  )
)
set logFile=!logDir!\%SWP_LOCALTIME_DATESTAMP%_!solutionName!.log

REM check which visual studio should be used
set "devenvCom=%VS110COMNTOOLS%\..\IDE\devenv.com"
if not exist "!devenvCom!" set "devenvCom=%VS120COMNTOOLS%\..\IDE\devenv.com"


REM call devenv.com
echo Call: "!devenvCom!" "!-solution!" !-additionalOptions! "!-configuration!|AnyCPU" /out "!logFile!"
call "!devenvCom!" "!-solution!" !-additionalOptions! "!-configuration!|AnyCPU" /out "!logFile!"
if errorlevel 1 ( 
  echo !devenvCom! failed.
  set !-Script.Name!.Errorlevel=100
) else (
  echo !devenvCom! succedded.
  
)


IF DEFINED CALLED_FROM_EXPLORER pause
exit /b !%-Script.Name%.Errorlevel!
