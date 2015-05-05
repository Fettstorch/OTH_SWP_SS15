@echo off
SETLOCAL EnableDelayedExpansion

cls
call "%~dp0\SetEnv.bat" > NUL

set configsToBuild=Release Debug
set /a internalErrorLevel=0

rem run builds
echo.
echo ===== Build GraphFramework and Usecase Tool:
for %%i in (!configsToBuild!) do (
  call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunBuild.bat" -configuration="%%i" -solution="%SWP_SRC_ROOT%/src.sln" -additionalOptions="/rebuild"
)

REM echo.
REM echo ===== Run GraphFramework UnitTests:
rem run unit tests
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunUnitTest.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\GraphFramework.Tests.dll"

rem run unit tests coverage
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunUnitTestCoverage.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\GraphFramework.Tests.dll"

rem create coverage report
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunReportGenerator.bat" -reports="%SWP_BUILD_ROOT%\UnitTest\%SWP_LOCALTIME_DATESTAMP%_GraphFramework.Tests_OpenCover.xml"

echo.
echo ===== Run UseCase Tool UnitTests:
rem run unit tests
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunUnitTest.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\UseCaseAnalyser.Model.Tests.dll"

rem run unit tests coverage
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunUnitTestCoverage.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\UseCaseAnalyser.Model.Tests.dll"

rem create coverage report
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunReportGenerator.bat" -reports="%SWP_BUILD_ROOT%\UnitTest\%SWP_LOCALTIME_DATESTAMP%_UseCaseAnalyser.Model.Tests_OpenCover.xml"



rem create static code analysis reports
echo.
echo ===== Run GraphFramework Static Code Analysis:
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunFxCop.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\GraphFramework.dll" -outDir="%SWP_BUILD_ROOT%/StaticCodeAnalysis/FxCop"
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunFxCop.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\GraphFramework.Visualiser.dll" -outDir="%SWP_BUILD_ROOT%/StaticCodeAnalysis/FxCop"

echo.
echo ===== Run UseCase Tool Static Code Analysis:
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunFxCop.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\UseCaseAnalyser.Model.dll" -outDir="%SWP_BUILD_ROOT%/StaticCodeAnalysis/FxCop"

echo.
echo ===== Run Logger Static Code Analysis:
call :call_and_check_errorlevel "%SWP_SCRIPT_ROOT%\RunFxCop.bat" -target="%SWP_SRC_OUTDIR_RELEASE%\LogManager.dll" -outDir="%SWP_BUILD_ROOT%/StaticCodeAnalysis/FxCop"


echo. 
if /I "%internalErrorLevel%" EQU "0" (
	echo ALL WENT WELL - READY FOR CHECK-IN!
) else (
	echo ERROR OCCURRED - PLEASE INVESTIGATE AND FIX ERROR BEFORE CHECK-IN!
)

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