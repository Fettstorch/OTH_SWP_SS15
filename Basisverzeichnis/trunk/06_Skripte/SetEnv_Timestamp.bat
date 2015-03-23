@echo off
echo Setup time environment variables.

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"

set "SWP_LOCALTIME_DATESTAMP=%YYYY%_%MM%_%DD%" 
set "SWP_LOCALTIME_TIMESTAMP=%HH%-%Min%-%Sec%"
set "SWP_LOCALTIME_FULLSTAMP=%SWP_LOCALTIME_DATESTAMP%_%SWP_LOCALTIME_TIMESTAMP%"

REM output all Ruby relevant environmental variables:
set SWP_LOCALTIME_

