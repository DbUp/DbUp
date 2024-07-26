# Getting Started

1. Clone the `DbUp/Universe` repository: `git clone https://github.com/DbUp/Universe`
1. Run the `CloneAllProviders.ps1` script in that directory, this will clone all the repositories as siblings of the `Universe` repository
1. Setup a variable with a GitHub Token that has access to packages: `$ghToken = Read-Host "Enter your GitHub token"`
1. Add the GitHub DbUp Nuget Source: ` dotnet nuget add source --name DbUp --username DbUp --password $ghToken https://nuget.pkg.github.com/DbUp/index.json`
1. Either open the solution in the `/src` folder of the relevant repo, or open `Universe/Universe.sln` to open all of them
