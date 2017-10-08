@echo off

if "%~1"=="" ( 
 call :Usage
 goto :EOF
)

pushd "%~dp0"
setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

rem Use vswhere to find MsBuild (https://github.com/Microsoft/vswhere/wiki/Find-MSBuild)

set VsWhere="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"

for /f "usebackq tokens=*" %%i in (`%VsWhere% -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" ResourceLib.proj /t:%*
  if NOT %ERRORLEVEL%==0 exit /b %ERRORLEVEL%
)

popd
endlocal
exit /b 0
goto :EOF

:Usage
echo  Syntax:
echo.
echo   build [target] /p:Configuration=[Debug (default),Release]
echo.
echo  Target:
echo.
echo   all : clean, build code, run tests, generate docs, package into zip (requires Sandcastle and Sandcastle Builder)
echo   code : clean, build code
echo   code_and_test : clean, build code, run tests
echo   run_test_only : run tests
echo.
echo  Examples:
echo.
echo   build all
echo   build all /p:Configuration=Release
goto :EOF
