#tool "nuget:?package=GitReleaseNotes"
#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var outputDir = "./artifacts/";

void UpdateProjectJsonVersion(string projectName) {
    var proj = string.Format("./src/{0}/project.json", projectName);
    var updatedProjectJson = System.IO.File.ReadAllText(proj)
        .Replace("1.0.0-*", versionInfo.NuGetVersion);

    System.IO.File.WriteAllText(proj, updatedProjectJson);
}

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore();
    });

GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
        UpdateProjectJsonVersion("dbup-core");
        UpdateProjectJsonVersion("dbup-firebird");
        UpdateProjectJsonVersion("dbup-mysql");
        UpdateProjectJsonVersion("dbup-postgresql");
        UpdateProjectJsonVersion("dbup-sqlce");
        UpdateProjectJsonVersion("dbup-sqlite");
        UpdateProjectJsonVersion("dbup-sqlite-mono");
        UpdateProjectJsonVersion("dbup-sqlserver");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        MSBuild("./src/DbUp.sln");
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest("./src/dbup-tests");
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = outputDir,
            NoBuild = true
        };

        DotNetCorePack("./src/dbup-core/project.json", settings);
        DotNetCorePack("./src/dbup-firebird/project.json", settings);
        DotNetCorePack("./src/dbup-mysql/project.json", settings);
        DotNetCorePack("./src/dbup-postgresql/project.json", settings);
        DotNetCorePack("./src/dbup-sqlce/project.json", settings);
        DotNetCorePack("./src/dbup-sqlite/project.json", settings);
        DotNetCorePack("./src/dbup-sqlite-mono/project.json", settings);
        DotNetCorePack("./src/dbup-sqlserver/project.json", settings);

        var releaseNotesExitCode = StartProcess(
            @"tools\GitReleaseNotes\tools\gitreleasenotes.exe", 
            new ProcessSettings { Arguments = ". /o artifacts/releasenotes.md" });
        if (string.IsNullOrEmpty(System.IO.File.ReadAllText("./artifacts/releasenotes.md")))
            System.IO.File.WriteAllText("./artifacts/releasenotes.md", "No issues closed since last release");

        if (releaseNotesExitCode != 0) throw new Exception("Failed to generate release notes");

        System.IO.File.WriteAllLines(outputDir + "artifacts", new[]
        {
            "core:dbup-core." + versionInfo.NuGetVersion + ".nupkg",
            "firebird:dbup-firebird." + versionInfo.NuGetVersion + ".nupkg",
            "mysql:dbup-mysql." + versionInfo.NuGetVersion + ".nupkg",
            "postgresql:dbup-postgresql." + versionInfo.NuGetVersion + ".nupkg",
            "sqlce:dbup-sqlce." + versionInfo.NuGetVersion + ".nupkg",
            "sqlite:dbup-sqlite." + versionInfo.NuGetVersion + ".nupkg",
            "sqlite-mono:dbup-sqlite-mono." + versionInfo.NuGetVersion + ".nupkg",
            "sqlserver:dbup-sqlserver." + versionInfo.NuGetVersion + ".nupkg",
            "releaseNotes:releasenotes.md"
        });

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*"))
                AppVeyor.UploadArtifact(file.FullPath);
        }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);