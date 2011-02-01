$buildFile = "build\Build.proj" ;
$framework = "v4.0.30319" ;
$fxPath = Join-Path -Path "C:\Windows\Microsoft.NET\Framework\" -ChildPath $framework ;
$msbuildPath = Join-Path -Path $fxPath -ChildPath "MSBuild.exe"

& $msbuildpath $buildfile /t:Build /v:m
