@echo off
echo Setup user related environment variables.

rem setup all team member
set SWP_USER_NAMES=Schiessl_Patrick Poelloth_Stefan Betting_Pascal Mertz_Sebastian Schlemelch_Manuel Weigl_Benjamin Hegen_Erik Saalfrank_Adrian Schneider_Mathias
set SWP_USER_GITHUB_NAMES=Fettstorch monsterspace Coder-Luke Parsen07 Ratatoeskr DasAddition r0oto0r SebMertz

rem map GitHub name to user name
set "SWP_USER_Coder-Luke.NAME=Schiessl_Patrick"
set "SWP_USER_SebMertz.NAME=Mertz_Sebastian"
set "SWP_USER_monsterspace.NAME=Schlemelch_Manuel"
set "SWP_USER_Parsen07.NAME=Betting_Pascal"
set "SWP_USER_Ratatoeskr.NAME=Hegen_Erik"
set "SWP_USER_DasAddition.NAME=Saalfrank_Adrian"
set "SWP_USER_r0oto0r.NAME=Weigl_Benjamin"
set "SWP_USER_Fettstorch.NAME=Schneider_Mathias"
set "SWP_USER_SPoelloth.NAME=Poelloth_Stefan"


rem setup all team member uni login needed for SVN login
set "SWP_USER_Schiessl_Patrick.UNI_LOGIN=cbb0"
set "SWP_USER_Mertz_Sebastian.UNI_LOGIN=e5b1"
set "SWP_USER_Betting_Pascal.UNI_LOGIN=92bd"
set "SWP_USER_Schlemelch_Manuel.UNI_LOGIN=7920"
set "SWP_USER_Weigl_Benjamin.UNI_LOGIN=e9cc"
set "SWP_USER_Hegen_Erik.UNI_LOGIN=62a8"
set "SWP_USER_Saalfrank_Adrian.UNI_LOGIN=b9d0"
set "SWP_USER_Schneider_Mathias.UNI_LOGIN=aa46"
set "SWP_USER_Poelloth_Stefan.UNI_LOGIN=cad1"

set "SWP_USER_SVN_LOGIN_PASSWORD=swp15"


REM output all GitHub relevant environmental variables:
set SWP_USER

