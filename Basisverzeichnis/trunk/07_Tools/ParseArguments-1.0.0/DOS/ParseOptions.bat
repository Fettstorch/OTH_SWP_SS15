@echo off
IF NOT DEFINED OptionDefaults (
  echo %~nx0 is used by a calling script to parse its arguments in a common and easily readable manner. The list of arguments may contain:
  echo  1. Up to one unnamed argument. Its value will be stored in variable -unnamed.
  echo  2. Any number of options with values. The values will be saved in variables named after the option.
  echo     e.g.: If the option is called "-retry" and the script is called with "-retry=3", then after calling %~nx0 the variable -retry will contain the value 3.
  echo  3. Any number of switches without values. As a switch doesn't have a value, the calling script may check for it with "IF DEFINED -option ...".
  echo 
  echo Usage: First set default values for all named options and switches your script recognizes in variable OptionDefaults, then call this Script with the arguments passed to your script.
  echo Notes:
  echo   - %~nx0 always requires OptionDefaults to be set. If your script does not recognize any named arguments, there is no reason to call %~nx0 in the first place.
  echo   - Switches are defined in OptionDefaults by providing no value after the colon. The colon is mandatory.
  echo   - If an option with value should be undefined by default, use quotes.
  echo   - For techincal reasons, names and values in OptionDefaults are seperated by a ':', but have to be seperated by '=' when calling the script.
  echo Example:
  echo     set "OptionDefaults=-file-in:name.ext -file-out:"" -retry:3 -silent:"
  echo     call %~nx0 %%^*
  exit /B 1
)


:: Set the default values for named arguments
for %%O in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%A in ("%%O") do set "%%A=%%~B"
set -unnamed=
set "OptionDefaults=!OptionDefaults:"='! "

setlocal EnableDelayedExpansion
:: Validate and store the options, one at a time, using a loop.
:loop

  :: leave loop when there are no more parameters to handle
  set name=%~1
  if not defined name goto :break
  shift /1

  if "!OptionDefaults:%name%:=!" == "!OptionDefaults!" (
    rem Parameter is not recognized as name of an option (not found in "OptionDefaults" string)
    rem this is either an error, or an unnamed parameter.
    if "!name:~0,1!"=="-" (
      echo Error: Invalid option "%name%"
      exit /b 1
    ) else if not defined -unnamed (
      rem this version of the script allows for exactly one unnamed parameter
      endlocal & set "-unnamed=%name%"
      setlocal EnableDelayedExpansion
    ) else (
      echo Error: Too many unnamed parameters, only one allowed
      exit /b 1
    )
  ) else if "!OptionDefaults:%name%: =!" == "!OptionDefaults!" (
    rem The option provided expects a value. Set the option value using the option as the name.
    rem and the next arg as the value
    endlocal & set "%name%=%~1"
    setlocal EnableDelayedExpansion
    shift /1
  ) else (
    rem The option provided expects no value. Set the flag option using the option name.
    rem The value doesn't matter, it just needs to be defined.
    endlocal & set "%name%=TRUE"
    setlocal EnableDelayedExpansion
  )
goto :loop
:break

goto :EOF