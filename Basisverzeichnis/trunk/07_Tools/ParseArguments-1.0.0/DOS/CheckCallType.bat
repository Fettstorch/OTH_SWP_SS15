@echo off
setlocal EnableDelayedExpansion

REM cmdcmdline contains the exact commandline the current prompt was created with.
set cmdline=!cmdcmdline!

REM remove quotes from cmdcmdline variable that mess with using its value in an IF clause
set cmdline=!cmdline:"=!

IF NOT DEFINED CHECKED_CALL_TYPE (
  if /I "!cmdline:cmd.exe /c=!" NEQ "!cmdline!" (
    endlocal & SET CALLED_FROM_EXPLORER=TRUE& SET CALLED_FROM_CMD=
  ) else (
    endlocal & SET CALLED_FROM_CMD=TRUE& SET CALLED_FROM_EXPLORER=
  )
  endlocal & set CHECKED_CALL_TYPE=True
)