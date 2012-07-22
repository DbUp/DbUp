@echo off

set framework=v4.0.30319

"%SystemDrive%\Windows\Microsoft.NET\Framework\%framework%\MSBuild.exe" build\Build.proj /t:Build /v:m /p:build_number=2.0.0
echo Done
pause
