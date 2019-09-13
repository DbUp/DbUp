using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;
using DbUp.Tests.TestInfrastructure;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests
{
    [Story(
         AsA = "As a DbUp User",
         IWant = "I want to DbUp to upgrade my database to the latest version using my own sort order",
         SoThat = "So that my application's database is up to date")]
    public class RunGroupOrderScenarios
    {
        private readonly List<SqlScript> scripts;
        private readonly UpgradeEngineBuilder upgradeEngineBuilder;
        private readonly CaptureLogsLogger logger;
        private readonly DelegateConnectionFactory testConnectionFactory;
        private readonly RecordingDbConnection recordingConnection;
        private DatabaseUpgradeResult upgradeResult;
        private UpgradeEngine upgradeEngine;

        public RunGroupOrderScenarios()
        {
            upgradeResult = null;
            scripts = new List<SqlScript>
            {
                new SqlScript("ZZZScript1.sql", "create table Foo (Id int identity)", new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder}),
                new SqlScript("ZZZScript2.sql", "alter table Foo add column Name varchar(255)", new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder}),
                new SqlScript("AAAScript3.sql", "insert into Foo (Name) values ('test')", new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder + 1})
            };

            logger = new CaptureLogsLogger();
            recordingConnection = new RecordingDbConnection(logger, "SchemaVersions");
            testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);

            upgradeEngineBuilder = DeployChanges.To
                .SqlDatabase("testconn")
                .WithScripts(new TestScriptProvider(scripts))
                .OverrideConnectionFactory(testConnectionFactory)
                .LogTo(logger);
        }

        [Fact]
        public void UpgradingAnOutOfDateDatabase()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldHaveSuccessfulResult())
                .And(t => t.AndShouldHaveRunAllScriptsInOrder())
                .And(t => t.AndShouldLogInformation())
                .BDDfy();
        }

        private void AndShouldLogInformation()
        {
            logger.InfoMessages.ShouldContain("Beginning database upgrade");
            logger.InfoMessages.ShouldContain("Upgrade successful");
        }

        private void AndShouldHaveRunAllScriptsInOrder()
        {
            // Check both results and journal
            upgradeResult.Scripts
                .Select(s => s.Name)
                .ShouldBe(new[] { "ZZZScript1.sql", "ZZZScript2.sql", "AAAScript3.sql" });
        }

        private void ThenShouldHaveSuccessfulResult()
        {
            upgradeResult.Successful.ShouldBeTrue();
        }

        private void GivenAnOutOfDateDatabase()
        {
        }

        private void WhenDatabaseIsUpgraded()
        {
            upgradeEngine = upgradeEngineBuilder.Build();
            upgradeResult = upgradeEngine.PerformUpgrade();
        }

        public class TestScriptProvider : IScriptProvider
        {
            private readonly List<SqlScript> sqlScripts;

            public TestScriptProvider(List<SqlScript> sqlScripts)
            {
                this.sqlScripts = sqlScripts;
            }

            public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
            {
                return sqlScripts;
            }
        }
    }
}