@echo off
SETLOCAL EnableDelayedExpansion

cls
call "%~dp0\SetEnv.bat" > NUL

set configsToBuild=Release Debug
set /a internalErrorLevel=0

rem run builds
for %%i in (!configsToBuild!) do (
  call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunBuild.bat" -configuration="%%i" -solution="%SWP_SRC_ROOT%/src/NUnitTestLib/NUnitTestLib.sln" -additionalOptions="/rebuild"
)

rem run unit tests
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunUnitTest.bat" -target="%SWP_SRC_ROOT%\src\NUnitTestLib\NUnitTestLib\bin\Release\NUnitTestLib.dll"

rem run unit tests coverage
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunUnitTestCoverage.bat" -target="%SWP_SRC_ROOT%\src\NUnitTestLib\NUnitTestLib\bin\Release\NUnitTestLib.dll"

rem create coverage report
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunReportGenerator.bat" -reports="%SWP_BRANCH_ROOT%\Build\UnitTest\NUnitTestLib\Log\%SWP_LOCALTIME_DATESTAMP%_NUnitTestLib_OpenCover.xml"

rem create static code analysis report
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunFxCop.bat" -target="%SWP_SRC_ROOT%\src\NUnitTestLib\NUnitTestLib\bin\Release\NUnitTestLib.dll" -outDir="%SWP_BRANCH_ROOT%/Build/StaticCodeAnalysis/FxCop"

exit /b %internalErrorLevel%


REM :call_and_check_errorlevel <COMMAND>
:call_and_check_errorlevel <COMMAND>
for %%F in ("%~1") do set "cmdName=%%~nF"
echo=============================================================================================
echo Call !cmdName!:

if /I "%internalErrorLevel%" EQU "0" (
  echo.
  call %*
  if errorlevel 1 (
    set /a internalErrorLevel+=1
    echo !cmdName! failed.
  ) else (
    echo.
  )
) else (
  echo  Skipped due to former error.
)
exit /b 0