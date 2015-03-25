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
Wrapper for github_issues_to_csv.rb script.^
""

REM configure ParseArguments
set "OptionDefaults=-userName:"" -password:"" -organization:"" -repository:"" -outFile:"" -openOutFile:"

set member=!SWP_GITHUB_OTH_SWP_SS15_MEMBER: =^|!

REM ===========================================================================================
REM configure ParseOptions.bat GUI interface
REM ===========================================================================================
if not defined -userName.Usage              set "-userName.Usage="Select GitHub user name.""
if not defined -userName.Necessity          set "-userName.Necessity="Required""
if not defined -userName.Multiplicity       set "-userName.Multiplicity="Single""
if not defined -userName.GuiEntryType       set "-userName.GuiEntryType="Default""
if not defined -userName.Values             set "-userName.Values="!member!""
if not defined -userName.GuiDefaultValues   set "-userName.GuiDefaultValues="Fettstorch""

if not defined -password.Usage              set "-password.Usage="Enter password - be careful it will not be masked. If you do not enter password here, you will be asked on command line for it again, but this time without any output shown.""
if not defined -password.Necessity          set "-password.Necessity="Required""
if not defined -password.GuiEntryType       set "-password.GuiEntryType="Password""
if not defined -password.GuiDefaultValues   set "-password.GuiDefaultValues="""

if not defined -organization.Usage              set "-organization.Usage="Select GitHub organization.""
if not defined -organization.Necessity          set "-organization.Necessity="Required""
if not defined -organization.Multiplicity       set "-organization.Multiplicity="Single""
if not defined -organization.GuiDefaultValues   set "-organization.GuiDefaultValues="%SWP_GITHUB_ORGANIZATION_NAME%""

if not defined -repository.Usage              set "-repository.Usage="Select GitHub repository.""
if not defined -repository.Necessity          set "-repository.Necessity="Required""
if not defined -repository.Multiplicity       set "-repository.Multiplicity="Single""
if not defined -repository.GuiDefaultValues   set "-repository.GuiDefaultValues="%SWP_GITHUB_REPOSITORY_NAME%""

if not defined -outFile.Usage              set "-outFile.Usage="Select resulting file name (Ruby script will add extension)""
if not defined -outFile.Necessity          set "-outFile.Necessity="Required""
if not defined -outFile.GuiEntryType       set "-outFile.GuiEntryType="File""
if not defined -outFile.Multiplicity       set "-outFile.Multiplicity="Single""
if not defined -outFile.GuiDefaultValues   set "-outFile.GuiDefaultValues="%SWP_BRANCH_ROOT%\Documentation\Issues\%SWP_LOCALTIME_DATESTAMP%_%SWP_GITHUB_REPOSITORY_NAME%_Issues""

if not defined -openOutFile.Usage              set "-openOutFile.Usage="Open issues file using Notepad++Portable after successful migration.""
if not defined -openOutFile.Necessity          set "-openOutFile.Necessity="Optional""
if not defined -openOutFile.Multiplicity       set "-openOutFile.Multiplicity="Bool""
if not defined -openOutFile.GuiEntryType       set "-openOutFile.GuiEntryType="Default""
if not defined -openOutFile.GuiDefaultValues   set "-openOutFile.GuiDefaultValues="Disabled""

call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

rem output selected options
echo !-Script.Name!
for %%A in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%B in ("%%A") do (
  set name=!%%B!
  if /I "!%%B!" NEQ "" echo %%B=!name!
)
echo.

if not exist "%SWP_BRANCH_ROOT%\Documentation\Issues" mkdir "%SWP_BRANCH_ROOT%\Documentation\Issues"

echo Call: %SWP_RUBY_EXE% %SWP_GITHUB_TO_CSV_EXE% "!-userName! !-organization! !-repository! !-outFile! !-password!"
if errorlevel 1 (
  echo %SWP_GITHUB_TO_CSV_EXE% failed.
) else (
  echo %SWP_GITHUB_TO_CSV_EXE% succedded.
  if defined -openOutFile (
    taskkill /IM notepad*
    call %SWP_NOTEPAD++PORTABLE_EXE% !-outFile!.txt
  )
)

IF DEFINED CALLED_FROM_EXPLORER IF NOT DEFINED -openOutFile pause
