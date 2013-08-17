using System;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.ScriptProviders;
using NUnit.Framework;

namespace DbUp.Tests
{
    public class EmbeddedScriptAndCodeProviderTests : SpecificationFor<EmbeddedScriptAndCodeProvider>
    {
        private SqlScript[] scriptsToExecute;

        public override EmbeddedScriptAndCodeProvider Given()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return new EmbeddedScriptAndCodeProvider(assembly, s=>true);
        }

        public override void When()
        {
            var testConnectionManager = new TestConnectionManager();
            testConnectionManager.UpgradeStarting(new ConsoleUpgradeLog());
            scriptsToExecute = Subject.GetScripts(testConnectionManager).ToArray();
        }

        [Then]
        public void it_should_return_all_sql_files()
        {
            Assert.AreEqual(4, scriptsToExecute.Length);
        }

        [Then]
        public void should_provide_content_for_code_script()
        {
            Assert.AreEqual("test4", scriptsToExecute.Last().Contents);
        }
    }
}