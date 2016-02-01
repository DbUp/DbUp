param($installPath, $toolsPath, $package)
 
foreach ($_ in Get-Module | ?{$_.Name -eq 'DbUp'})
{
    Remove-Module 'VSExtensionsModule'
}
      
Import-Module (Join-Path $toolsPath DbUp.psm1)
