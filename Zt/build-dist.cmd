@echo off
setlocal

cd "%~dp0"

For %%a in (
"Zt\bin\Release\net48\*.dll"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtCreator\bin\Release\net48\*.dll"
"ZtCreator\bin\Release\net48\*.exe"
"ZtCreator\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtPatcher\bin\Release\net48\*.dll"
"ZtPatcher\bin\Release\net48\*.exe"
"ZtPatcher\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtBlank\bin\Release\net48\*.dll"
"ZtBlank\bin\Release\net48\*.exe"
"ZtBlank\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)
