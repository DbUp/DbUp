#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var outputDir = "./artifacts/";

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    });



GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    });

Task("Restore")
    .IsDependentOn("Version")
    .Does(() => {
        DotNetCoreRestore("src", new DotNetCoreRestoreSettings() {
            ArgumentCustomization = args => args.Append("/p:Version=" + versionInfo.NuGetVersion)
        });
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        var settings =  new MSBuildSettings()
            .SetConfiguration("Release")
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("Version", versionInfo.NuGetVersion)
            .WithProperty("PackageOutputPath", System.IO.Path.GetFullPath(outputDir))
            .WithTarget("Build")
            .WithTarget("Pack");

        MSBuild("./src/DbUp.sln", settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
         DotNetCoreTest("./src/dbup-tests/dbup-tests.csproj", new DotNetCoreTestSettings
        {
            Configuration = "Release",
            NoBuild = true
        });
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {

        NuGetPack("./src/dbup/dbup.nuspec", new NuGetPackSettings() {
            OutputDirectory = System.IO.Path.GetFullPath(outputDir),
            Version = versionInfo.NuGetVersion
        });

        System.IO.File.WriteAllLines(outputDir + "artifacts", new[]
        {
            "core:dbup-core." + versionInfo.NuGetVersion + ".nupkg",
            "firebird:dbup-firebird." + versionInfo.NuGetVersion + ".nupkg",
            "mysql:dbup-mysql." + versionInfo.NuGetVersion + ".nupkg",
            "postgresql:dbup-postgresql." + versionInfo.NuGetVersion + ".nupkg",
            "redshift:dbup-redshift." + versionInfo.NuGetVersion + ".nupkg",
            "sqlce:dbup-sqlce." + versionInfo.NuGetVersion + ".nupkg",
            "sqlite:dbup-sqlite." + versionInfo.NuGetVersion + ".nupkg",
            "sqlite-mono:dbup-sqlite-mono." + versionInfo.NuGetVersion + ".nupkg",
            "sqlserver:dbup-sqlserver." + versionInfo.NuGetVersion + ".nupkg",
            "sqlanywhere:dbup-sqlanywhere." + versionInfo.NuGetVersion + ".nupkg"
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