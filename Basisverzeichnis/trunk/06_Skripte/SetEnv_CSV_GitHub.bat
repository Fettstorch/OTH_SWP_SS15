@echo off
echo Setup CSV-GitHub-import-export environment variables.

set "SWP_CSV_GITHUB_IMPORT_EXPORT_ROOT=%SWP_SCRIPT_ROOT%/CSV-GitHub-import-export"

set "SWP_GITHUB_TO_CSV_EXE=%SWP_CSV_GITHUB_IMPORT_EXPORT_ROOT%/github_issues_to_csv.rb"
set "SWP_CSV_TO_GITHUB_EXE=%SWP_CSV_GITHUB_IMPORT_EXPORT_ROOT%/csv_issues_to_github.rb"

REM output all CSV-GitHub-import-export relevant environmental variables:
set SWP_CSV_
set SWP_GITHUB_
