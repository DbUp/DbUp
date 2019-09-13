using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.ScriptProviders;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;

namespace DbUp.Tests.ScriptProvider
{
    public class EmbeddedScriptAndCodeProviderTests
    {
        public class when_no_specific_filter_is_set : SpecificationFor<EmbeddedScriptAndCodeProvider>
        {
            SqlScript[] scriptsToExecute;

            public override EmbeddedScriptAndCodeProvider Given()
            {
                var assembly = typeof(EmbeddedScriptAndCodeProviderTests).GetTypeInfo().Assembly;

                return new EmbeddedScriptAndCodeProvider(assembly, s => true);
            }

            protected override void When()
            {
                var testConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>());
                testConnectionManager.OperationStarting(new ConsoleUpgradeLog(), new List<SqlScript>());
                scriptsToExecute = Subject.GetScripts(testConnectionManager).ToArray();
            }

            [Then]
            public void it_should_return_all_sql_files()
            {
                scriptsToExecute
                    .Count(s => s.Name.EndsWith(".sql"))
                    .ShouldBe(9);
            }

            [Then]
            public void should_ignore_abstract_implementations()
            {
                scriptsToExecute
                    .Where(s => s.Name.EndsWith("Script20120723_1_Test4_Base.cs"))
                    .ShouldBeEmpty();
            }

            [Then]
            public void should_provide_content_for_code_script()
            {
                scriptsToExecute
                    .Single(s => s.Name.EndsWith("Script20120723_1_Test4.cs"))
                    .Contents
                    .ShouldBe("test4");
            }
        }

        public class when_a_specific_filter_is_set : SpecificationFor<EmbeddedScriptAndCodeProvider>
        {
            SqlScript[] scriptsToExecute;

            public override EmbeddedScriptAndCodeProvider Given()
            {
                var assembly = Assembly.GetExecutingAssembly();

                return new EmbeddedScriptAndCodeProvider(assembly, s => !s.Contains("Test4"));
            }

            protected override void When()
            {
                var testConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>());
                testConnectionManager.OperationStarting(new ConsoleUpgradeLog(), new List<SqlScript>());
                scriptsToExecute = Subject.GetScripts(testConnectionManager).ToArray();
            }

            [Then]
            public void should_not_return_the_code_based_script()
            {
                scriptsToExecute.ShouldNotContain(x => x.Name.Contains("Test4"));
            }

            [Then]
            public void it_should_only_return_the_sql_scripts()
            {
                scriptsToExecute.Length.ShouldBe(9);
            }
        }
    }
}
