@echo off
REM setlocal enabledelayedexpansion

echo Setup all tool relevant environment variables.

set "SWP_SCRIPT_ROOT=%~dp0"
set "SWP_BRANCH_ROOT=%SWP_SCRIPT_ROOT%.."
pushd .
cd /d %SWP_BRANCH_ROOT%
set SWP_BRANCH_ROOT=%CD%
popd

set "SWP_TOOLS_ROOT=%SWP_BRANCH_ROOT%/07_Tools"
set "SWP_SRC_ROOT=%SWP_BRANCH_ROOT%/03_Implementierung"

set "setEnvPattern=%SWP_SCRIPT_ROOT%SetEnv_*.bat"
for /r %%i in (SetEnv_*.bat) do call "%%i"


