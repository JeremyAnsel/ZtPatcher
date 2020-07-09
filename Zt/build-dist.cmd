@echo off
setlocal

cd "%~dp0"

For %%a in (
"Zt\bin\Release\net40\*.dll"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtCreator\bin\Release\net40\*.dll"
"ZtCreator\bin\Release\net40\*.exe"
"ZtCreator\bin\Release\net40\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtPatcher\bin\Release\net40\*.dll"
"ZtPatcher\bin\Release\net40\*.exe"
"ZtPatcher\bin\Release\net40\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"ZtBlank\bin\Release\net40\*.dll"
"ZtBlank\bin\Release\net40\*.exe"
"ZtBlank\bin\Release\net40\*.config"
) do (
xcopy /s /d "%%~a" dist\
)
