@echo off


echo Setup Notepad++Portable environment variables.

set "SWP_NOTEPAD++PORTABLE_VERSION_MAJOR=2"
set "SWP_NOTEPAD++PORTABLE_VERSION_MINOR=0"
set "SWP_NOTEPAD++PORTABLE_VERSION_SUBMINOR=0"

set "SWP_NOTEPAD++PORTABLE_VERSION=%SWP_NOTEPAD++PORTABLE_VERSION_MAJOR%.%SWP_NOTEPAD++PORTABLE_VERSION_MINOR%.%SWP_NOTEPAD++PORTABLE_VERSION_SUBMINOR%"

set "SWP_NOTEPAD++PORTABLE_ROOT=%SWP_TOOLS_ROOT%/Notepad++Portable-%SWP_NOTEPAD++PORTABLE_VERSION%/"

set "SWP_NOTEPAD++PORTABLE_EXE=%SWP_NOTEPAD++PORTABLE_ROOT%/Notepad++Portable.exe"

REM output all ParseArguments relevant environmental variables:
set SWP_NOTEPAD++PORTABLE
