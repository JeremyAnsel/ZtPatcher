@echo off
setlocal

cd "%~dp0"

For %%a in (
"Zt\bin\Release\*.dll"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtCreator\bin\Release\*.dll"
"ZtCreator\bin\Release\*.exe"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtPatcher\bin\Release\*.dll"
"ZtPatcher\bin\Release\*.exe"
) do (
xcopy /s /d "%%~a" dist\
)
