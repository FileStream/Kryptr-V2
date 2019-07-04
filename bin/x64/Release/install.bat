@echo off
assoc .kv2=kryptrfile
ftype kryptrfile="%~dp0Kryptr V2.exe" "%%1"
cls
echo[
echo   Installation successful
echo[
pause