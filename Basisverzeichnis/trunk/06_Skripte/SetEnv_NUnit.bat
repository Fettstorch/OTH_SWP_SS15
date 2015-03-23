@echo off

echo Setup NUnit environment variables.

set "SWP_NUNIT_VERSION_MAJOR=2"
set "SWP_NUNIT_VERSION_MINOR=6"
set "SWP_NUNIT_VERSION_SUBMINOR=4"

set "SWP_NUNIT_VERSION=%SWP_NUNIT_VERSION_MAJOR%.%SWP_NUNIT_VERSION_MINOR%.%SWP_NUNIT_VERSION_SUBMINOR%"

set "SWP_NUNIT_ROOT=%SWP_TOOLS_ROOT%/NUnit-%SWP_NUNIT_VERSION%/"

set "SWP_NUNIT_EXE=%SWP_NUNIT_ROOT%/bin/nunit.exe"
set "SWP_NUNIT_CMD_EXE=%SWP_NUNIT_ROOT%/bin/nunit-console.exe"


REM output all NUnit relevant environmental variables:
set SWP_NUNIT

