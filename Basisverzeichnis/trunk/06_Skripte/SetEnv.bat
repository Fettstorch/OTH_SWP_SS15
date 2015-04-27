@echo off
REM setlocal enabledelayedexpansion

echo Setup all tool relevant environment variables.

set "SWP_SCRIPT_ROOT=%~dp0"
set "SWP_BRANCH_ROOT=%SWP_SCRIPT_ROOT%.."
pushd .
cd /d %SWP_BRANCH_ROOT%
set SWP_BRANCH_ROOT=%CD%
popd

REM setup sources directory and define outdir
set "SWP_SRC_ROOT=%SWP_BRANCH_ROOT%/03_Implementierung/src"

set "SWP_BUILD_ROOT=%SWP_SRC_ROOT%/Build"

set "SWP_SRC_OUTDIR_RELEASE=%SWP_BUILD_ROOT%/bin/Release"
set "SWP_SRC_OUTDIR_DEBUG=%SWP_BUILD_ROOT%/Build/bin/Debug"
set "SWP_SRC_OUTDIR=%SWP_SRC_OUTDIR_RELEASE%"


set "SWP_TOOLS_ROOT=%SWP_BRANCH_ROOT%/07_Tools"




set "setEnvPattern=%SWP_SCRIPT_ROOT%SetEnv_*.bat"
for /r %%i in (SetEnv_*.bat) do call "%%i"


