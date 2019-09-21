using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Helpers
{
    public class FilterFactoryTests
    {
        [Fact(Skip = "Need to come up with a better way than current directory")]
        public void Should_Exclude_ScriptNames_Listed_In_File()
        {
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            var excludeFile = System.IO.Path.Combine(currentDir, "TestFilterFiles", "ScriptNames.txt");
            var filter = Filters.ExcludeScriptNamesInFile(excludeFile);

            var testScripts = new List<string>
            {
                "Script20110301_1_Test1.txt",
                "ShouldRemain.txt",
                "Script20130525_1_Test5.txt"
            };

            var scriptsToRun = testScripts.Where(filter);

            scriptsToRun.ShouldBe(new[] { "ShouldRemain.txt" });
        }

        [Fact(Skip = "Need to come up with a better way than current directory")]
        public void Should_Include_Only_ScriptNames_Listed_In_File()
        {
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            var excludeFile = System.IO.Path.Combine(currentDir, "TestFilterFiles", "ScriptNames.txt");
            var filter = Filters.OnlyIncludeScriptNamesInFile(excludeFile);

            var testScripts = new List<string>
            {
                "Script20110301_1_Test1.txt",
                "ShouldNotRemain.txt",
                "Script20130525_1_Test5.txt"
            };

            var scriptsToRun = testScripts.Where(filter);

            scriptsToRun.ShouldNotBeNull();
            scriptsToRun.Count().ShouldBe(2);
            scriptsToRun.ShouldNotContain("ShouldNotRemain.txt");
        }

        [Fact]
        public void Should_Exclude_ScriptNames()
        {
            var testScripts = new List<string>
            {
                "Script20110301_1_Test1.txt",
                "ShouldRemain.txt",
                "Script20130525_1_Test5.txt"
            };

            var filter = Filters.ExcludeScripts("Script20110301_1_Test1.txt", "Script20130525_1_Test5.txt");
            var scriptsToRun = testScripts.Where(filter);

            scriptsToRun.ShouldNotBeNull();
            scriptsToRun.Count().ShouldBe(1);
            scriptsToRun.First().ShouldBe("ShouldRemain.txt");
        }

        [Fact]
        public void Should_Include_ScriptNames()
        {
            var testScripts = new List<string>
            {
                "Script20110301_1_Test1.txt",
                "ShouldNotRemain.txt",
                "Script20130525_1_Test5.txt"
            };

            var filter = Filters.OnlyIncludeScripts("Script20110301_1_Test1.txt", "Script20130525_1_Test5.txt");
            var scriptsToRun = testScripts.Where(filter);

            scriptsToRun.ShouldNotBeNull();
            scriptsToRun.Count().ShouldBe(2);
            scriptsToRun.Contains("ShouldNotRemain.txt").ShouldBeFalse();
        }

    }
}
