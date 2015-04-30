param($installPath, $toolsPath, $package, $project)

    # Add the SQLite SDK reference
	$sqliteReference = $project.Object.References.AddSDK("SQLite for Windows Runtime (Windows 8.1)", "SQLite.WinRT81, version=3.8.7.4")
    Write-Host "Successfully added a reference to the extension SDK SQLite for Windows Runtime (Windows 8.1)."
    Write-Host "Please, verify that the extension SDK SQLite for Windows Runtime (Windows 8.1) v3.8.7.4, from the SQLite.org site (http://www.sqlite.org/2014/sqlite-winrt81-3080704.vsix), has been properly installed."

    # This is the global install file
    $rootInstall = [System.IO.Path]::Combine($toolsPath, '../install.ps1')
    $rootToolsPath = [System.IO.Path]::Combine($toolsPath, '../')
    Write-Host $rootInstall
    . $rootInstall -installPath $installPath -toolsPath $rootToolsPath -package $package -project $project
 