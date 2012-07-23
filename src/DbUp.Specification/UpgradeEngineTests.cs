using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Support.SqlServer;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification
{
    public class UpgradeEngineTests
    {
        public class when_upgrading_a_database_with_variable_substitution : SpecificationFor<UpgradeEngine>
        {
            private IJournal versionTracker;
            private IScriptProvider scriptProvider;
            private IScriptExecutor scriptExecutor;
            private IDbConnection dbConnection;
            private IDbCommand dbCommand;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScripts(Arg.Any<Func<IDbConnection>>()).Returns(new List<SqlScript> { new SqlScript("1234", "foo") });
                versionTracker = Substitute.For<IJournal>();
                dbConnection = Substitute.For<IDbConnection>();
                dbCommand = Substitute.For<IDbCommand>();
                dbConnection.CreateCommand().Returns(dbCommand);
                scriptExecutor = new SqlScriptExecutor(()=>dbConnection, ()=>new TraceUpgradeLog(), null, null);

                var builder = new UpgradeEngineBuilder()
                    .WithScript(new SqlScript("1234", "create table $var$ (Id int)"))
                    .JournalTo(versionTracker)
                    .WithVariable("var", "sub");
                builder.Configure(c => c.ScriptExecutor = scriptExecutor);
                builder.Configure(c => c.ConnectionFactory = () => dbConnection);

                var upgrader = builder.Build();
                return upgrader;
            }

            public override void When()
            {
                Subject.PerformUpgrade();
            }

            [Then]
            public void substitutes_variable()
            {
                Assert.AreEqual(dbCommand.CommandText, "create table sub (Id int)");
            }
        }

        public class when_marking_scripts_as_read : SpecificationFor<UpgradeEngine>
        {
            private IJournal versionTracker;
            private IScriptProvider scriptProvider;
            private IScriptExecutor scriptExecutor;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScripts(Arg.Any<Func<IDbConnection>>()).Returns(new List<SqlScript> { new SqlScript("1234", "foo") });
                versionTracker = Substitute.For<IJournal>();
                scriptExecutor = Substitute.For<IScriptExecutor>();

                var config = new UpgradeConfiguration();
                config.ScriptProviders.Add(scriptProvider);
                config.ScriptExecutor = scriptExecutor;
                config.Journal = versionTracker;

                var upgrader = new UpgradeEngine(config);
                return upgrader;
            }

            public override void When()
            {
                Subject.MarkAsExecuted();
            }

            [Then]
            public void the_scripts_are_journalled()
            {
                versionTracker.Received().StoreExecutedScript(Arg.Is<SqlScript>(s => s.Name == "1234"));
            }

            [Then]
            public void the_scripts_are_not_run()
            {
                scriptExecutor.DidNotReceiveWithAnyArgs().Execute(null);
            }
        }
    }
}