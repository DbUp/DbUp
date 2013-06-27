using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SQLite;
using DbUp.SQLite.Helpers;
using DbUp.Support.SQLite;
using NSubstitute;
using NUnit.Framework;
using TestStack.BDDfy;
using TestStack.BDDfy.Core;
using TestStack.BDDfy.Scanners.StepScanners.Fluent;

namespace DbUp.Tests.Specifications
{
    [TestFixture]
    [Story(
        AsA = "As a DbUp User",
        IWant = "I want to DbUp to upgrade my database to the latest version",
        SoThat = "So that my application's database is up to date")]
    public class UpgradeDatabase
    {
        private DatabaseUpgradeResult upgradeResult;
        private TemporarySQLiteDatabase database;
        private List<SqlScript> scripts;
        private UpgradeEngine upgradeEngine;
        private IUpgradeLog log;
        private UpgradeEngineBuilder upgradeEngineBuilder;

        [SetUp]
        public void SetUp()
        {
            log = Substitute.For<IUpgradeLog>();
            upgradeResult = null;
            scripts = new List<SqlScript>
            {
                new SqlScript("Script1.sql", "create table Foo (Id int identity)"),
                new SqlScript("Script2.sql", "alter table Foo add column Name varchar(255)"),
                new SqlScript("Script3.sql", "insert into Foo (Name) values ('test')")
            };
            database = new TemporarySQLiteDatabase("IntegrationScenarios");
            upgradeEngineBuilder = DeployChanges.To
                .SQLiteDatabase(database.ConnectionString)
                .WithScripts(new TestScriptProvider(scripts))
                .LogTo(log);
        }

        [TearDown]
        public void TearDown()
        {
            database.Dispose();
        }

        [Test]
        public void AttemptingToUpgradeAnUptoDateDatabase()
        {
            this.Given(t => t.GivenAnUpToDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenUpgradeShouldNotBeRequired())
                .And(t => t.AndShouldNotRunAnyScripts())
                .And(t => t.AndShouldHaveSuccessfulResult())
                .BDDfy();
        }

        [Test]
        public void UpgradingAnOutOfDateDatabase()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenUpgradeShouldNotBeRequired())
                .And(t => t.AndShouldHaveSuccessfulResult())
                .And(t => t.AndShouldHaveRunAllScripts())
                .And(t => t.AndShouldLogInformation())
                .BDDfy();
        }

        private void AndShouldLogInformation()
        {
            log.Received().WriteInformation("Beginning database upgrade");
            log.Received().WriteInformation("Upgrade successful");
        }

        private void AndShouldHaveRunAllScripts()
        {
            Assert.AreEqual(3, upgradeResult.Scripts.Count());
            Assert.AreEqual(3, GetJournal().GetExecutedScripts().Count());
        }

        private void AndShouldHaveSuccessfulResult()
        {
            Assert.IsTrue(upgradeResult.Successful);
        }

        private void GivenAnOutOfDateDatabase()
        {
            upgradeEngine = upgradeEngineBuilder.Build();
        }

        private void GivenAnUpToDateDatabase()
        {
            var journal = GetJournal();
            journal.StoreExecutedScript(scripts[0]);
            journal.StoreExecutedScript(scripts[1]);
            journal.StoreExecutedScript(scripts[2]);

            upgradeEngine = upgradeEngineBuilder.Build();
        }

        private SQLiteTableJournal GetJournal()
        {
            var sqLiteConnectionManager = new SQLiteConnectionManager(database.ConnectionString);
            sqLiteConnectionManager.UpgradeStarting(log);
            var journal = new SQLiteTableJournal(() => sqLiteConnectionManager, () => log, "SchemaVersions");
            return journal;
        }

        private void WhenDatabaseIsUpgraded()
        {
            upgradeResult = upgradeEngine.PerformUpgrade();
        }

        public void ThenUpgradeShouldNotBeRequired()
        {
            Assert.IsFalse(upgradeEngine.IsUpgradeRequired());
        }

        public void AndShouldNotRunAnyScripts()
        {
            Assert.AreEqual(0, upgradeResult.Scripts.Count());
        }

        public void ShouldReturnSuccess()
        {
            Assert.IsTrue(upgradeResult.Successful);
        }

        public void ShouldLogNoAction()
        {
            log.Received().WriteInformation("Beginning database upgrade");
            log.Received().WriteInformation("No new scripts need to be executed - completing.");
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
