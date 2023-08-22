$url = "https://download.microsoft.com/download/F/F/D/FFDF76E3-9E55-41DA-A750-1798B971936C/ENU/SSCERuntime_x64-ENU.exe"
$WebClient = New-Object System.Net.WebClient
$WebClient.DownloadFile($url ,"$env:TEMP/SSCERuntime_x64-ENU.exe");

Start-Process "$env:TEMP/SSCERuntime_x64-ENU.exe" -argumentlist "/i /quiet" -wait