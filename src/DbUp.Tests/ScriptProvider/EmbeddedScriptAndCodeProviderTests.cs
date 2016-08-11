using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.ScriptProviders;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.ScriptProvider
{
    public class when_no_specific_filter_is_set : SpecificationFor<EmbeddedScriptAndCodeProvider>
    {
        private SqlScript[] scriptsToExecute;

        public override EmbeddedScriptAndCodeProvider Given()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return new EmbeddedScriptAndCodeProvider(assembly, s => true);
        }

        public override void When()
        {
            var testConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>());
            testConnectionManager.OperationStarting(new ConsoleUpgradeLog(), new List<SqlScript>());
            scriptsToExecute = Subject.GetScripts(testConnectionManager).ToArray();
        }

        [Then]
        public void it_should_return_all_sql_files()
        {
            Assert.AreEqual(6, scriptsToExecute.Length);
        }

        [Then]
        public void should_provide_content_for_code_script()
        {
            Assert.AreEqual("test4", scriptsToExecute.Single(s => s.Name.EndsWith("Script20120723_1_Test4.cs")).Contents);
        }
    }
    public class when_a_specific_filter_is_set : SpecificationFor<EmbeddedScriptAndCodeProvider>
    {
        private SqlScript[] scriptsToExecute;

        public override EmbeddedScriptAndCodeProvider Given()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return new EmbeddedScriptAndCodeProvider(assembly, s => !s.Contains("Test4"));
        }

        public override void When()
        {
            var testConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>());
            testConnectionManager.OperationStarting(new ConsoleUpgradeLog(), new List<SqlScript>());
            scriptsToExecute = Subject.GetScripts(testConnectionManager).ToArray();
        }

        [Then]
        public void should_not_return_the_code_based_script()
        {
            Assert.False(scriptsToExecute.Any(x => x.Name.Contains("Test4")));
        }

        [Then]
        public void it_should_only_return_the_sql_scripts()
        {
            Assert.AreEqual(5, scriptsToExecute.Length);
        }
    }
}