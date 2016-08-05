#if !NETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SQLite;
using DbUp.SQLite.Helpers;
using NSubstitute;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests
{
    //TODO Use recording connection rather than Sqlite
    [Story(
        AsA = "As a DbUp User",
        IWant = "I want to DbUp to upgrade my database to the latest version",
        SoThat = "So that my application's database is up to date")]
    public class UpgradeDatabaseScenarios : IDisposable
    {
        DatabaseUpgradeResult upgradeResult;
        TemporarySQLiteDatabase database;
        List<SqlScript> scripts;
        UpgradeEngine upgradeEngine;
        IUpgradeLog log;
        UpgradeEngineBuilder upgradeEngineBuilder;
        bool isUpgradeRequired;

        public UpgradeDatabaseScenarios()
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

        [Fact]
        public void AttemptingToUpgradeAnUptoDateDatabase()
        {
            this.Given(t => t.GivenAnUpToDateDatabase())
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenShouldNotRunAnyScripts())
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
                .When(t => t.WhenDatabaseIsUpgraded())
                .Then(t => t.ThenUpgradeShouldNotBeRequired())
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

        [Fact]
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

        [Fact]
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

        void AndErrorMessageShouldBeLogged()
        {
            log.Received().WriteError("Script block number: {0}; Error code {1}; Message: {2}", 0, 1,
                "SQL logic error or missing database\r\n" +
                "near \"slect\": syntax error");

            log.Received().WriteError(Arg.Is<string>(s => s.StartsWith("System.Data.SQLite.SQLiteException (0x80004005): SQL logic error or missing database")));
            log.Received().WriteError(
                Arg.Is<string>(s => s.StartsWith("Upgrade failed due to an unexpected exception:")),
                Arg.Is<string>(s => s.Contains("System.Data.SQLite.SQLiteException")));
        }

        void ConfiguredToUseTransaction()
        {
            upgradeEngineBuilder.WithTransaction();
        }

        void AndShouldHaveFailedResult()
        {
            upgradeResult.Successful.ShouldBeFalse("Upgrade should not be successful");
        }

        void AndTheFourthScriptToRunHasAnError()
        {
            scripts.Add(new SqlScript("ScriptWithError.sql", "slect * from Oops"));
        }

        void AndTheScriptToRunHasAnError()
        {
            scripts.Clear();
            scripts.Add(new SqlScript("ScriptWithError.sql", "slect * from Oops"));
        }

        public void Dispose()
        {
            database.Dispose();
        }

        void AndShouldLogInformation()
        {
            log.Received().WriteInformation("Beginning database upgrade");
            log.Received().WriteInformation("Upgrade successful");
        }

        void AndShouldHaveRunAllScriptsInOrder()
        {
            // Check both results and journal
            upgradeResult.Scripts.Select(s => s.Name)
                .ShouldBe(new[] {"Script1.sql", "Script2.sql", "Script3.sql"});
            GetJournal().GetExecutedScripts().Count().ShouldBe(3);
        }

        void ThenShouldNotRunAnyScripts()
        {
            upgradeResult.Scripts.ShouldBeEmpty();
        }

        void ThenShouldHaveSuccessfulResult()
        {
            upgradeResult.Successful.ShouldBeTrue();
        }

        void GivenAnOutOfDateDatabase()
        {
        }

        void GivenAnUpToDateDatabase()
        {
            var journal = GetJournal();
            journal.StoreExecutedScript(scripts[0], () => database.SharedConnection.CreateCommand());
            journal.StoreExecutedScript(scripts[1], () => database.SharedConnection.CreateCommand());
            journal.StoreExecutedScript(scripts[2], () => database.SharedConnection.CreateCommand());
        }

        SQLiteTableJournal GetJournal()
        {
            var sqLiteConnectionManager = new SQLiteConnectionManager(database.SharedConnection);
            sqLiteConnectionManager.OperationStarting(log, new List<SqlScript>());
            var journal = new SQLiteTableJournal(() => sqLiteConnectionManager, () => log, "SchemaVersions");
            return journal;
        }

        void WhenCheckIfDatabaseUpgradeIsRequired()
        {
            upgradeEngine = upgradeEngineBuilder.Build();
            isUpgradeRequired = upgradeEngine.IsUpgradeRequired();
        }

        void WhenDatabaseIsUpgraded()
        {
            upgradeEngine = upgradeEngineBuilder.Build();
            upgradeResult = upgradeEngine.PerformUpgrade();
        }

        public void ThenUpgradeShouldNotBeRequired()
        {
            isUpgradeRequired.ShouldBeFalse();
        }

        void ThenUpgradeShouldBeRequired()
        {
            isUpgradeRequired.ShouldBeTrue();
        }

        public void ShouldReturnSuccess()
        {
            upgradeResult.Successful.ShouldBeTrue();
        }

        public void ShouldLogNoAction()
        {
            log.Received().WriteInformation("Beginning database upgrade");
            log.Received().WriteInformation("No new scripts need to be executed - completing.");
        }

        public class TestScriptProvider : IScriptProvider
        {
            readonly List<SqlScript> sqlScripts;

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
#endif