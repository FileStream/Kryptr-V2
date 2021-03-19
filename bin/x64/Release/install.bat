@echo off
SET /P INSTALLPATH=Enter an installation path, or leave blank to install in ProgramFiles: 
IF NOT DEFINED INSTALLPATH (
set INSTALLPATH = %PROGRAMFILES%
)
IF NOT EXIST "%INSTALLPATH%\Kryptr V2" (
mkdir "%INSTALLPATH%\Kryptr V2"
)
IF NOT EXIST "%INSTALLPATH%\Kryptr V2" (
echo Failed to create directory at install location.
echo[
pause
exit
)
copy "%~dp0" "%INSTALLPATH%\Kryptr V2"
cd "%PROGRAMFILES%\Kryptr V2"
assoc .kv2=kryptrfile
ftype kryptrfile="%INSTALLPATH%\Kryptr V2\KryptrGUI.exe" "%%1"
cls
echo Done.
echo[
pause