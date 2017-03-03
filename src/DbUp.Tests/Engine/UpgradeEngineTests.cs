using System.Collections.Generic;
using System.Data;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Engine
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
                scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript> { new SqlScript("1234", "foo") });
                versionTracker = Substitute.For<IJournal>();
                dbConnection = Substitute.For<IDbConnection>();
                dbCommand = Substitute.For<IDbCommand>();
                dbConnection.CreateCommand().Returns(dbCommand);
                var connectionManager = new TestConnectionManager(dbConnection);
                scriptExecutor = new SqlScriptExecutor(() => connectionManager, () => new TraceUpgradeLog(), null, () => true, null);

                var builder = new UpgradeEngineBuilder()
                    .WithScript(new SqlScript("1234", "create table $var$ (Id int)"))
                    .JournalTo(versionTracker)
                    .WithVariable("var", "sub");
                builder.Configure(c => c.ScriptExecutor = scriptExecutor);
                builder.Configure(c => c.ConnectionManager = connectionManager);

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
                Assert.AreEqual("create table sub (Id int)", dbCommand.CommandText);
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
                scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript> { new SqlScript("1234", "foo") });
                versionTracker = Substitute.For<IJournal>();
                scriptExecutor = Substitute.For<IScriptExecutor>();

                var config = new UpgradeConfiguration();
                config.ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>());
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