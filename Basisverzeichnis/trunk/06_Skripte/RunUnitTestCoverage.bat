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
Run NUnit module test for a target.^
""

set "!-Script.Name!.Errorlevel=0"

REM configure ParseArguments
set "OptionDefaults=-target:"" -logDir:"""


REM ===========================================================================================
REM configure ParseOptions.bat GUI interface
REM ===========================================================================================
if not defined -target.Usage              set "-target.Usage="Select target *.dll/*.exe which should be analyzed.""
if not defined -target.Necessity          set "-target.Necessity="Required""
if not defined -target.Multiplicity       set "-target.Multiplicity="Single""
if not defined -target.GuiEntryType       set "-target.GuiEntryType="File""
if not defined -target.Values             set "-target.Values="""
if not defined -target.GuiDefaultValues   set "-target.GuiDefaultValues="%SWP_BRANCH_ROOT%\03_Implementierung\src\NUnitTestLib\NUnitTest\bin\Debug\NUnitTestLib.dll"

if not defined -logDir.Usage              set "-logDir.Usage="Select root out directory for xml file. This directory wwill be expanded by target name.""
if not defined -logDir.Necessity          set "-logDir.Necessity="Optional""
if not defined -logDir.GuiEntryType       set "-logDir.GuiEntryType="Folder""
if not defined -logDir.Multiplicity       set "-logDir.Multiplicity="Single""
if not defined -logDir.GuiDefaultValues   set "-logDir.GuiDefaultValues="""

call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

rem output selected options
echo !-Script.Name!
for %%A in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%B in ("%%A") do (
  set name=!%%B!
  if /I "!%%B!" NEQ "" echo %%B=!name!
)
echo.

REM get plain target name
for %%F in ("!-target!") do set "targetName=%%~nF"

REM define log directory and log file
set logDir=!-logDir!
if not defined -logDir (
  set "logDir=%SWP_BRANCH_ROOT%\Build\!-configuration!\UnitTest\!targetName!\Log"
  if not exist !logDir! (
    echo Create !logDir!.
    mkdir "%SWP_BRANCH_ROOT%\Build\!-configuration!\UnitTest\!targetName!\Log"
  )
)
set logFileNUnit=!logDir!\%SWP_LOCALTIME_DATESTAMP%_!targetName!_NUnit.xml
set logFileOpenCover=!logDir!\%SWP_LOCALTIME_DATESTAMP%_!targetName!_OpenCover.xml
if exist !logFile! del !logFile!


echo Call: "%SWP_OPENCOVER_CMD_EXE%" -target:"%SWP_NUNIT_CMD_EXE%" -targetargs:"/nologo /noshadow /xml:\"!logFileNUnit!\" \"!-target!\"" -register:user -output:"!logFileOpenCover!"
echo.
call "%SWP_OPENCOVER_CMD_EXE%" -target:"%SWP_NUNIT_CMD_EXE%" -targetargs:"/nologo /noshadow /xml:\"!logFileNUnit!\" \"!-target!\"" -register:user -output:"!logFileOpenCover!"
if errorlevel 1 ( 
  echo %SWP_OPENCOVER_CMD_EXE% failed.
  set !-Script.Name!.Errorlevel=100
) else (
  echo %SWP_OPENCOVER_CMD_EXE% succedded.
)

IF DEFINED CALLED_FROM_EXPLORER pause
exit /b !%-Script.Name%.Errorlevel!