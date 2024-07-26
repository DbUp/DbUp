using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Helpers;

public class FilterFactoryTests
{
    [Fact]
    public void Should_Exclude_ScriptNames_Listed_In_File()
    {
        var tempExcludeFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempExcludeFile, @"Script20110301_1_Test1.txt
Script20130525_1_Test5.txt");

            var filter = Filters.ExcludeScriptNamesInFile(tempExcludeFile);

            var testScripts = new List<string> {"Script20110301_1_Test1.txt", "ShouldRemain.txt", "Script20130525_1_Test5.txt"};

            var scriptsToRun = testScripts.Where(filter);

            scriptsToRun.ShouldBe(new[] {"ShouldRemain.txt"});
        }
        finally
        {
            if (File.Exists(tempExcludeFile))
            {
                File.Delete(tempExcludeFile);
            }
        }
    }

    [Fact]
    public void Should_Include_Only_ScriptNames_Listed_In_File()
    {
        var tempIncludeFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempIncludeFile, @"Script20110301_1_Test1.txt
Script20130525_1_Test5.txt");

            var filter = Filters.OnlyIncludeScriptNamesInFile(tempIncludeFile);

            var testScripts = new List<string> {"Script20110301_1_Test1.txt", "ShouldNotRemain.txt", "Script20130525_1_Test5.txt"};

            var scriptsToRun = testScripts.Where(filter);

            scriptsToRun.ShouldNotBeNull();
            scriptsToRun.Count().ShouldBe(2);
            scriptsToRun.ShouldNotContain("ShouldNotRemain.txt");
        }
        finally
        {
            if (File.Exists(tempIncludeFile))
            {
                File.Delete(tempIncludeFile);
            }
        }
    }

    [Fact]
    public void Should_Exclude_ScriptNames()
    {
        var testScripts = new List<string> {"Script20110301_1_Test1.txt", "ShouldRemain.txt", "Script20130525_1_Test5.txt"};

        var filter = Filters.ExcludeScripts("Script20110301_1_Test1.txt", "Script20130525_1_Test5.txt");
        var scriptsToRun = testScripts.Where(filter);

        scriptsToRun.ShouldNotBeNull();
        scriptsToRun.Count().ShouldBe(1);
        scriptsToRun.First().ShouldBe("ShouldRemain.txt");
    }

    [Fact]
    public void Should_Include_ScriptNames()
    {
        var testScripts = new List<string> {"Script20110301_1_Test1.txt", "ShouldNotRemain.txt", "Script20130525_1_Test5.txt"};

        var filter = Filters.OnlyIncludeScripts("Script20110301_1_Test1.txt", "Script20130525_1_Test5.txt");
        var scriptsToRun = testScripts.Where(filter);

        scriptsToRun.ShouldNotBeNull();
        scriptsToRun.Count().ShouldBe(2);
        scriptsToRun.Contains("ShouldNotRemain.txt").ShouldBeFalse();
    }
}
