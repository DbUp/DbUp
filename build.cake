#tool "nuget:?package=GitVersion.CommandLine&Version=5.12.0"
#tool "nuget:?package=NuGet.CommandLine&version=6.7.0"
#addin "nuget:?package=Cake.Json&version=7.0.1"

var target = Argument("target", "Default");
var outputDir = "./artifacts/";

Task("Clean")
    .Does(() => {
        Information("ArtifactDirectory:");
        Information(outputDir);
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(
                outputDir,
                new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                }
            );
        }
        EnsureDirectoryExists(outputDir);
    });

GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
        string versionOutputDir = System.Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
        string versionOutputPath = "semver.txt";
        bool isGitHubAction = !string.IsNullOrWhiteSpace(versionOutputDir);
        if (isGitHubAction)
        {
            Information("ENV:GITHUB_OUTPUT:");
            Information(versionOutputDir);
        }
        else {
            versionOutputDir = System.IO.Path.GetFullPath(outputDir);
            EnsureDirectoryExists(versionOutputDir);
            versionOutputPath = System.IO.Path.Combine(versionOutputDir, versionOutputPath);
		}
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
        Information("SemVer:Version_Info:");
        Information(SerializeJsonPretty(versionInfo));
        Information("SemVer:Output_Path:");
        Information(versionOutputPath);
        Information("Writing Version_Info to file.");
        System.IO.File.WriteAllText(versionOutputPath, $"Version_Info_SemVer={versionInfo.SemVer}", Encoding.UTF8);
    });

Task("Restore")
    .IsDependentOn("Version")
    .Does(() => {
        DotNetRestore("src", new DotNetRestoreSettings() {
            ArgumentCustomization = args => args.Append("/p:Version=" + versionInfo.SemVer)
        });
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        var settings =  new MSBuildSettings()
            .SetConfiguration("Release")
            .WithProperty("Version", versionInfo.SemVer)
            .WithProperty("PackageOutputPath", System.IO.Path.GetFullPath(outputDir))
            .WithTarget("Build")
            .WithTarget("Pack");

        MSBuild("./src/DbUp.sln", settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
         DotNetTest("./src/dbup-tests/dbup-tests.csproj", new DotNetTestSettings
        {
            Configuration = "Release",
            NoBuild = true,
            Loggers = new[] {"console;verbosity=detailed", "trx" },
            ResultsDirectory = $"{outputDir}/TestResults"
        });
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {

        NuGetPack("./src/dbup/dbup.nuspec", new NuGetPackSettings() {
            OutputDirectory = System.IO.Path.GetFullPath(outputDir),
            Version = versionInfo.SemVer
        });
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);
