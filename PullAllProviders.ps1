$allProviders = Get-Content -Path AllProviders.txt

pushd

foreach ($provider in $allProviders) {
     Write-Host "Pulling $provider"
     cd ../$provider
     git pull
}

popd