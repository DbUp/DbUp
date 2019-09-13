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
         IWant = "I want to DbUp to upgrade my database to the latest version with a run always script",
         SoThat = "So that run always scripts always run and the run once script only run once")]
    public class ScriptTypeScenarios
    {
        private readonly List<SqlScript> scripts;
        private readonly UpgradeEngineBuilder upgradeEngineBuilder;
        private readonly CaptureLogsLogger logger;
        private readonly DelegateConnectionFactory testConnectionFactory;
        private readonly RecordingDbConnection recordingConnection;
        private DatabaseUpgradeResult upgradeResult;
        private UpgradeEngine upgradeEngine;
        private bool isUpgradeRequired;

        public ScriptTypeScenarios()
        {
            upgradeResult = null;
            scripts = new List<SqlScript>
            {
                new SqlScript("Script1.sql", "create table Foo (Id int identity)", new SqlScriptOptions { ScriptType = ScriptType.RunOnce}),
                new SqlScript("Script2.sql", "alter table Foo add column Name varchar(255)", new SqlScriptOptions { ScriptType = ScriptType.RunOnce}),
                new SqlScript("Script3.sql", "insert into Foo (Name) values ('test')", new SqlScriptOptions { ScriptType = ScriptType.RunAlways})
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
        public void AttemptingToUpgradeAnUptoDateDatabase()
        {
            this.Given(t => t.GivenAnUpToDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldHaveOnlyRunAlwaysScripts())
                .And(t => t.ThenShouldHaveSuccessfulResult())
                .BDDfy();
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

        [Fact]
        public void IsUpdateRequestedForAnUptoDateDatabase()
        {
            this.Given(t => t.GivenAnUpToDateDatabase())
                .When(t => t.WhenCheckIfDatabaseUpgradeIsRequired())
                .Then(t => t.ThenUpgradeShouldBeRequired())
                .BDDfy();
        }

        [Fact]
        public void IsUpdateRequestedForOutOfDateDatabase()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .When(t => t.WhenCheckIfDatabaseUpgradeIsRequired())
                .Then(t => t.ThenUpgradeShouldBeRequired())
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
                .ShouldBe(new[] { "Script1.sql", "Script2.sql", "Script3.sql" });
        }

        private void ThenShouldHaveOnlyRunAlwaysScripts()
        {
            upgradeResult.Scripts.Select(s => s.Name).ShouldBe(new[] { "Script3.sql" });
        }

        private void ThenShouldHaveSuccessfulResult()
        {
            upgradeResult.Successful.ShouldBeTrue();
        }

        private void GivenAnOutOfDateDatabase()
        {
        }

        private void GivenAnUpToDateDatabase()
        {
            recordingConnection.SetupRunScripts(scripts[0], scripts[1], scripts[2]);
        }

        private void WhenCheckIfDatabaseUpgradeIsRequired()
        {
            upgradeEngine = upgradeEngineBuilder.Build();
            isUpgradeRequired = upgradeEngine.IsUpgradeRequired();
        }

        private void WhenDatabaseIsUpgraded()
        {
            upgradeEngine = upgradeEngineBuilder.Build();
            upgradeResult = upgradeEngine.PerformUpgrade();
        }

        private void ThenUpgradeShouldNotBeRequired()
        {
            isUpgradeRequired.ShouldBeFalse();
        }

        private void ThenUpgradeShouldBeRequired()
        {
            isUpgradeRequired.ShouldBeTrue();
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