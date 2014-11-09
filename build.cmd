::Copied from https://github.com/fsprojects/VisualFSharpPowerTools/blob/master/build.cmd
:: Apache 2 License.

@echo off
cls

.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

packages\FAKE\tools\FAKE.exe build.fsx %*
pause