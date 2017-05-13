using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DbUp.Tests.Helpers
{
    [TestFixture]
    public class FilterFactoryTests
    {
        [Test]
        public void Should_Exclude_ScriptNames_Listed_In_File()
        {
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            var excludeFile = System.IO.Path.Combine(currentDir, "TestFilterFiles", "ScriptNames.txt");
            var filter = Filters.ExcludeScriptNamesInFile(excludeFile);

            var testScripts = new List<string>();
            testScripts.Add("Script20110301_1_Test1.txt");
            testScripts.Add("ShouldRemain.txt");
            testScripts.Add("Script20130525_1_Test5.txt");

            var scriptsToRun = testScripts.Where(filter);

            Assert.That(scriptsToRun, Is.Not.Null);
            Assert.That(scriptsToRun.Count(), Is.EqualTo(1));
            Assert.That(scriptsToRun.First(), Is.EqualTo("ShouldRemain.txt"));
        }

        [Test]
        public void Should_Include_Only_ScriptNames_Listed_In_File()
        {
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            var excludeFile = System.IO.Path.Combine(currentDir, "TestFilterFiles", "ScriptNames.txt");
            var filter = Filters.OnlyIncludeScriptNamesInFile(excludeFile);

            var testScripts = new List<string>();
            testScripts.Add("Script20110301_1_Test1.txt");
            testScripts.Add("ShouldNotRemain.txt");
            testScripts.Add("Script20130525_1_Test5.txt");

            var scriptsToRun = testScripts.Where(filter);

            Assert.That(scriptsToRun, Is.Not.Null);
            Assert.That(scriptsToRun.Count(), Is.EqualTo(2));
            Assert.False(scriptsToRun.Contains("ShouldNotRemain.txt"));
        }

        [Test]
        public void Should_Exclude_ScriptNames()
        {
            var testScripts = new List<string>();
            testScripts.Add("Script20110301_1_Test1.txt");
            testScripts.Add("ShouldRemain.txt");
            testScripts.Add("Script20130525_1_Test5.txt");

            var filter = Filters.ExcludeScripts("Script20110301_1_Test1.txt", "Script20130525_1_Test5.txt");
            var scriptsToRun = testScripts.Where(filter);

            Assert.That(scriptsToRun, Is.Not.Null);
            Assert.That(scriptsToRun.Count(), Is.EqualTo(1));
            Assert.That(scriptsToRun.First(), Is.EqualTo("ShouldRemain.txt"));
        }

        [Test]
        public void Should_Include_ScriptNames()
        {        
          
            var testScripts = new List<string>();
            testScripts.Add("Script20110301_1_Test1.txt");
            testScripts.Add("ShouldNotRemain.txt");
            testScripts.Add("Script20130525_1_Test5.txt");

            var filter = Filters.OnlyIncludeScripts("Script20110301_1_Test1.txt", "Script20130525_1_Test5.txt");
            var scriptsToRun = testScripts.Where(filter);

            Assert.That(scriptsToRun, Is.Not.Null);
            Assert.That(scriptsToRun.Count(), Is.EqualTo(2));
            Assert.False(scriptsToRun.Contains("ShouldNotRemain.txt"));
        }

    }
}
