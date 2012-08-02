$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$buildFile = "build\Build.proj"
$scriptRoot = ($MyInvocation.MyCommand.Path | Split-Path | Resolve-Path).ProviderPath
$absoluteBuildFile = $scriptRoot | Join-Path -ChildPath $buildFile

$fxPath = (Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full").InstallPath

$msbuildPath = Join-Path -Path $fxPath -ChildPath "MSBuild.exe"

& $msbuildpath $absoluteBuildfile /t:Build /v:m

if (-not $?) { throw "MSBuild failed with exit code $LastExitCode" }
