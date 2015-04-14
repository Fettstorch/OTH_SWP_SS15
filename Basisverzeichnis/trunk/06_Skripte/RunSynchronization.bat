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
set "OptionDefaults=-userName:"" -password:"" -organization:"" -repository:"" -destination:"""

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
if not defined -password.Necessity          set "-password.Necessity="Optional""
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

if not defined -destination.Usage              set "-destination.Usage="Select destination for synchronization.""
if not defined -destination.Necessity          set "-destination.Necessity="Required""
if not defined -destination.GuiEntryType       set "-destination.GuiEntryType="Folder""
if not defined -destination.Multiplicity       set "-destination.Multiplicity="Single""
if not defined -destination.GuiDefaultValues   set "-destination.GuiDefaultValues=""C:\Users\MathiasSchneider\Desktop\SWP_Neu\SVN_PseudoClient\trunk"""



call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

if not defined -destination set -destination="C:\Users\MathiasSchneider\Desktop\SWP_Neu\SVN_PseudoClient\trunk"

rem output selected options
echo !-Script.Name!
for %%A in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%B in ("%%A") do (
  set name=!%%B!
  if /I "!%%B!" NEQ "" echo %%B=!name!
)
echo.

pushd .
echo Synchronization directory: %SWP_GITHUB_GIT_LAST_SYNCHRONIZATION_COMMIT%
cd /d "%SWP_GITHUB_GIT_LAST_SYNCHRONIZATION_COMMIT%"
set lastCommitID=
for %%i in (*) do set "lastCommitID=%%i"

REM get plain target name
for %%F in ("!lastCommitID!") do set "lastCommitID=%%~nF"
popd

if not exist "%~dp0\Synchronization\Logs\" mkdir "%~dp0\Synchronization\Logs\"

REM get all commit since last synchronization
set "tmpGitCommits=%~dp0\Synchronization\Logs\%SWP_LOCALTIME_DATESTAMP%_GitCommits.txt"

echo Call: "%SWP_GITHUB_GIT_EXE%" log --reverse --pretty=fuller !lastCommitID!..HEAD
call "%SWP_GITHUB_GIT_EXE%" log --reverse --pretty=fuller !lastCommitID!..HEAD > "!tmpGitCommits!"

echo.

REM now parse last commits
set commitID=
set commitAuthor=
set commitDate=
set commitComment=

for /f "tokens=*" %%a in (!tmpGitCommits!) do (
	REM assume empty line is indicator for new commit.
	if /I "%%a" EQU "" (
		rem skip
	) else (
		set currentLine=%%a
		if /I "!currentLine!" NEQ "!currentLine:commit =!" (
			call :get_git_revision
			set commitID=!currentLine:commit =!
		) else if /I "!currentLine!" NEQ "!currentLine:commitDate: =!" (
			set commitDate=!currentLine:commitDate: =!
		) else if /I "!currentLine!" NEQ "!currentLine:Commit: =!" (
			set commitAuthor=!currentLine:Commit: =!
		)	else if /I "!currentLine!" NEQ "!currentLine:Author: =!" (
			REM unused data
		) else if /I "!currentLine!" NEQ "!currentLine:AuthorDate: =!" (
			REM unused data
		) else (
			set commitComment=!commitComment! !currentLine!

		)
	)
)

REM update last commit
call :get_git_revision


REM update last commit ID



IF DEFINED CALLED_FROM_EXPLORER pause
exit /b 0


:get_git_revision
if defined commitID (
	set "commitVars=commitID commitAuthor commitDate commitComment0"

	if defined commitID for /f "tokens=* delims= " %%a in ("!commitID!") do set commitID=%%a
	if defined commitAuthor for /f "tokens=* delims= " %%a in ("!commitAuthor!") do set commitAuthor=%%a
	if defined commitDate for /f "tokens=* delims= " %%a in ("!commitDate!") do set commitDate=%%a
	if defined commitComment for /f "tokens=* delims= " %%a in ("!commitComment!") do set commitComment=%%a

	set validCommitData=True
	for %%i in (!commitVars!) do (
		if not defined %%i set validCommitData= 
	)

	if defined validCommitData (
		echo.
		echo Commit data:
		echo commitID: !commitID!
		echo commitAuthor: !commitAuthor!
		echo commitDate: !commitDate!
		echo commitComment: !commitComment!
		
	  REM get revision at current commit
		call "%SWP_GITHUB_GIT_EXE%" checkout !commitID!
		
		REM pause
		
		REM robocopy it to SVN directory
		
		pushd .
		robocopy "%SWP_BRANCH_ROOT%" "!-destination!" *.* /S /XD Build
		
		pause
		REM commit change to SVN server
		REM call "%SWP_SLIKSVN_CMD%" 
		
		popd
		
		
	) else (
		echo Corrupted commit data found.
	)



	set commitID=
	set commitAuthor=
	set commitDate=
	set commitComment=
)
exit /b 0





