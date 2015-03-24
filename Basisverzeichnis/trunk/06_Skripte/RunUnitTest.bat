@echo off
SETLOCAL EnableDelayedExpansion

call "%~dp0\SetEnv.bat"

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
if not defined -target.GuiDefaultValues   set "-target.GuiDefaultValues="%SWP_NUNIT_ROOT%bin/tests/nunit-console.tests.dll""

if not defined -logDir.Usage              set "-logDir.Usage="Select root out directory for xml file. This directory wwill be expanded by target name.""
if not defined -logDir.Necessity          set "-logDir.Necessity="Optional""
if not defined -logDir.GuiEntryType       set "-logDir.GuiEntryType="Folder""
if not defined -logDir.Multiplicity       set "-logDir.Multiplicity="Single""
if not defined -logDir.GuiDefaultValues   set "-logDir.GuiDefaultValues="!FxCopDropLocation!""

call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

echo OPTIONS:
set -

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
set logFile=!logDir!\%SWP_LOCALTIME_DATESTAMP%_!targetName!.xml
if exist !logFile! del !logFile!


echo %SWP_NUNIT_CMD_EXE% !-target! /xml:!logFile!
%SWP_NUNIT_CMD_EXE% !-target! /xml:!logFile!
if errorlevel 1 ( 
  echo %SWP_NUNIT_CMD_EXE% failed.
  set !-Script.Name!.Errorlevel=100
) else (
  echo %SWP_NUNIT_CMD_EXE% succedded.
)

IF DEFINED CALLED_FROM_EXPLORER pause
exit /b !%-Script.Name%.Errorlevel!