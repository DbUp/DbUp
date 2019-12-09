using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SqlServer;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;

namespace DbUp.Tests.Engine
{
    public class UpgradeEngineTests
    {
        public class when_upgrading_a_database_with_variable_substitution : SpecificationFor<UpgradeEngine>
        {
            IJournal versionTracker;
            IScriptProvider scriptProvider;
            IScriptExecutor scriptExecutor;
            IDbConnection dbConnection;
            IDbCommand dbCommand;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript> { new SqlScript("1234", "foo") });
                versionTracker = Substitute.For<IJournal>();
                dbConnection = Substitute.For<IDbConnection>();
                dbCommand = Substitute.For<IDbCommand>();
                dbConnection.CreateCommand().Returns(dbCommand);
                var connectionManager = new TestConnectionManager(dbConnection);
                scriptExecutor = new SqlScriptExecutor(() => connectionManager, () => Substitute.For<IUpgradeLog>(), null, () => true, null, () => versionTracker);

                var builder = new UpgradeEngineBuilder()
                    .WithScript(new SqlScript("1234", "create table $var$ (Id int)"))
                    .JournalTo(versionTracker)
                    .WithVariable("var", "sub");
                builder.Configure(c => c.ScriptExecutor = scriptExecutor);
                builder.Configure(c => c.ConnectionManager = connectionManager);

                var upgrader = builder.Build();
                return upgrader;
            }

            protected override void When()
            {
                Subject.PerformUpgrade();
            }

            [Then]
            public void substitutes_variable()
            {
                dbCommand.CommandText.ShouldBe("create table sub (Id int)");
            }
        }

        public class when_marking_scripts_as_read : SpecificationFor<UpgradeEngine>
        {
            IJournal versionTracker;
            IScriptProvider scriptProvider;
            IScriptExecutor scriptExecutor;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript> { new SqlScript("1234", "foo") });
                versionTracker = Substitute.For<IJournal>();
                scriptExecutor = Substitute.For<IScriptExecutor>();

                var config = new UpgradeConfiguration
                {
                    ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())
                };
                config.ScriptProviders.Add(scriptProvider);
                config.ScriptExecutor = scriptExecutor;
                config.Journal = versionTracker;

                var upgrader = new UpgradeEngine(config);
                return upgrader;
            }

            protected override void When()
            {
                Subject.MarkAsExecuted();
            }

            [Then]
            public void the_scripts_are_journalled()
            {
                versionTracker.Received().StoreExecutedScript(Arg.Is<SqlScript>(s => s.Name == "1234"), Arg.Any<Func<IDbCommand>>());
            }

            [Then]
            public void the_scripts_are_not_run()
            {
                scriptExecutor.DidNotReceiveWithAnyArgs().Execute(null);
            }
        }

        public class when_querying_discovered_scripts : SpecificationFor<UpgradeEngine>
        {
            IJournal versionTracker;
            IScriptProvider scriptProvider;
            IScriptExecutor scriptExecutor;
            List<string> discoveredScripts;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                versionTracker = Substitute.For<IJournal>();
                versionTracker.GetExecutedScripts().Returns(new[] { "#1", "#2", "#3" });
                scriptExecutor = Substitute.For<IScriptExecutor>();

                var config = new UpgradeConfiguration
                {
                    ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())
                };
                config.ScriptProviders.Add(scriptProvider);
                config.ScriptExecutor = scriptExecutor;
                config.Journal = versionTracker;

                var upgrader = new UpgradeEngine(config);
                return upgrader;
            }

            protected override void When()
            {
                discoveredScripts = Subject.GetExecutedScripts();
            }

            [Then]
            public void discovered_scripts_are_returned()
            {
                discoveredScripts.ShouldBe(new[] { "#1", "#2", "#3" });
            }
        }

        public class when_querying_executed_but_not_discovered_scripts : SpecificationFor<UpgradeEngine>
        {
            IJournal versionTracker;
            IScriptProvider scriptProvider;
            IScriptExecutor scriptExecutor;
            List<string> discoveredScripts;

            public override UpgradeEngine Given()
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(new List<SqlScript>
                {
                    new SqlScript("#1", "Content of #1"),
                    new SqlScript("#3", "Content of #3"),
                });
                versionTracker = Substitute.For<IJournal>();
                versionTracker.GetExecutedScripts().Returns(new[] { "#1", "#2", "#3" });
                scriptExecutor = Substitute.For<IScriptExecutor>();

                var config = new UpgradeConfiguration
                {
                    ConnectionManager = new TestConnectionManager(Substitute.For<IDbConnection>())
                };
                config.ScriptProviders.Add(scriptProvider);
                config.ScriptExecutor = scriptExecutor;
                config.Journal = versionTracker;

                var upgrader = new UpgradeEngine(config);
                return upgrader;
            }

            protected override void When()
            {
                discoveredScripts = Subject.GetExecutedButNotDiscoveredScripts();
            }

            [Then]
            public void discovered_scripts_are_returned()
            {
                discoveredScripts.ShouldBe(new[] { "#2" });
            }
        }
    }
}
