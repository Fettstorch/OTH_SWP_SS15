@echo off
echo Setup Ruby environment variables.

set "SWP_RUBY_VERSION_MAJOR=2"
set "SWP_RUBY_VERSION_MINOR=0"
set "SWP_RUBY_VERSION_SUBMINOR=0"

set "SWP_RUBY_VERSION=%SWP_RUBY_VERSION_MAJOR%.%SWP_RUBY_VERSION_MINOR%.%SWP_RUBY_VERSION_SUBMINOR%"

set "SWP_RUBY_ROOT=%SWP_TOOLS_ROOT%/Ruby-%SWP_RUBY_VERSION%/"

set "SWP_RUBY_EXE=%SWP_RUBY_ROOT%/bin/ruby.exe"

REM output all Ruby relevant environmental variables:
set SWP_RUBY

