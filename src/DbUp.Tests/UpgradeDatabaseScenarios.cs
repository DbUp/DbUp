using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using NSubstitute;
using NUnit.Framework;
using TestStack.BDDfy;

namespace DbUp.Tests
{
    //TODO Use recording connection rather than Sqlite
    [TestFixture]
    [Story(
        AsA = "As a DbUp User",
        IWant = "I want to DbUp to upgrade my database to the latest version",
        SoThat = "So that my application's database is up to date")]
    public class UpgradeDatabaseScenarios
    {
        private DatabaseUpgradeResult upgradeResult;
        private TemporarySQLiteDatabase database;
        private List<SqlScript> scripts;
        private UpgradeEngine upgradeEngine;
        private IUpgradeLog log;
        private UpgradeEngineBuilder upgradeEngineBuilder;
        private bool isUpgradeRequired;

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
                .SQLiteDatabase(database.SharedConnection)
                .WithScripts(new TestScriptProvider(scripts))
                .LogTo(log);
        }

        [Test]
        public void AttemptingToUpgradeAnUptoDateDatabase()
        {
            this.Given(t => t.GivenAnUpToDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldNotRunAnyScripts())
                .And(t => t.ThenShouldHaveSuccessfulResult())
                .BDDfy();
        }

        [Test]
        public void UpgradingAnOutOfDateDatabase()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldHaveSuccessfulResult())
                .And(t => t.AndShouldHaveRunAllScriptsInOrder())
                .And(t => t.AndShouldLogInformation())
                .BDDfy();
        }

        [Test]
        public void IsUpdateRequestedForAnUptoDateDatabase()
        {
            this.Given(t => t.GivenAnUpToDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenUpgradeShouldNotBeRequired())
                .BDDfy();
        }

        [Test]
        public void IsUpdateRequestedForOutOfDateDatabase()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .When(t => t.WhenCheckIfDatabaseUpgradeIsRequired())
                .Then(t => t.ThenUpgradeShouldBeRequired())
                .BDDfy();
        }

        [Test]
        public void UpgradeDatabaseFails()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .And(t => t.AndTheScriptToRunHasAnError())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldNotRunAnyScripts())
                .And(t => t.AndShouldHaveFailedResult())
                .And(t => t.AndErrorMessageShouldBeLogged())
                .BDDfy();
        }

        [Test]
        public void UpgradeDatabaseFailsWithTransactionsEnabled()
        {
            this.Given(t => t.GivenAnOutOfDateDatabase())
                .And(t => t.AndTheFourthScriptToRunHasAnError())
                .And(t => t.ConfiguredToUseTransaction())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldNotRunAnyScripts())
                .And(t => t.AndShouldHaveFailedResult())
                .And(t => t.AndErrorMessageShouldBeLogged())
                .BDDfy();
        }

        private void AndErrorMessageShouldBeLogged()
        {
            log.Received().WriteError("Script block number: {0}; Error code {1}; Message: {2}", 0, 1,
                "SQL logic error or missing database\r\n" +
                "near \"slect\": syntax error");

            log.Received().WriteError(Arg.Is<string>(s => s.StartsWith("System.Data.SQLite.SQLiteException (0x80004005): SQL logic error or missing database")));
            log.Received().WriteError(
                Arg.Is<string>(s => s.StartsWith("Upgrade failed due to an unexpected exception:")),
                Arg.Is<string>(s => s.Contains("System.Data.SQLite.SQLiteException")));
        }

        private void ConfiguredToUseTransaction()
        {
            upgradeEngineBuilder.WithTransaction();
        }

        private void AndShouldHaveFailedResult()
        {
            Assert.IsFalse(upgradeResult.Successful, "Upgrade should not be successful");
        }

        private void AndTheFourthScriptToRunHasAnError()
        {
            scripts.Add(new SqlScript("ScriptWithError.sql", "slect * from Oops"));
        }

        private void AndTheScriptToRunHasAnError()
        {
            scripts.Clear();
            scripts.Add(new SqlScript("ScriptWithError.sql", "slect * from Oops"));
        }

        [TearDown]
        public void TearDown()
        {
            database.Dispose();
        }

        private void AndShouldLogInformation()
        {
            log.Received().WriteInformation("Beginning database upgrade");
            log.Received().WriteInformation("Upgrade successful");
        }

        private void AndShouldHaveRunAllScriptsInOrder()
        {
            // Check both results and journal
            Assert.AreEqual(3, upgradeResult.Scripts.Count());
            Assert.AreEqual("Script1.sql", upgradeResult.Scripts.ElementAt(0).Name);
            Assert.AreEqual("Script2.sql", upgradeResult.Scripts.ElementAt(1).Name);
            Assert.AreEqual("Script3.sql", upgradeResult.Scripts.ElementAt(2).Name);
            Assert.AreEqual(3, GetJournal().GetExecutedScripts().Count());
        }

        public void ThenShouldNotRunAnyScripts()
        {
            Assert.AreEqual(0, upgradeResult.Scripts.Count());
        }

        private void ThenShouldHaveSuccessfulResult()
        {
            Assert.IsTrue(upgradeResult.Successful);
        }

        private void GivenAnOutOfDateDatabase()
        {
        }

        private void GivenAnUpToDateDatabase()
        {
            var journal = GetJournal();
            journal.StoreExecutedScript(scripts[0]);
            journal.StoreExecutedScript(scripts[1]);
            journal.StoreExecutedScript(scripts[2]);
        }

        private SQLiteTableJournal GetJournal()
        {
            var sqLiteConnectionManager = new SQLiteConnectionManager(database.SharedConnection);
            sqLiteConnectionManager.OperationStarting(log, new List<SqlScript>());
            var journal = new SQLiteTableJournal(() => sqLiteConnectionManager, () => log, "SchemaVersions");
            return journal;
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

        public void ThenUpgradeShouldNotBeRequired()
        {
            Assert.IsFalse(isUpgradeRequired);
        }

        private void ThenUpgradeShouldBeRequired()
        {
            Assert.True(isUpgradeRequired);
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
