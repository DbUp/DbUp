using DbUp.Builder;
using DbUp.Engine;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;

namespace DbUp.Tests
{
    [TestFixture]
    public class TransactionScenarios
    {
        UpgradeEngineBuilder upgradeEngineBuilder;
        RecordingDbConnection testConnection;
        SqlScript[] scripts;

        [Test]
        public void UsingNoTransactionsScenario()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldExecuteScriptsWithoutUsingATransaction())
                .BDDfy();
        }

        [Test]
        public void UsingNoTransactionsScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldStopExecution())
                .BDDfy();
        }

        [Test]
        public void UsingTransactionPerScriptScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldHaveExecutedEachScriptInATransaction())
                .BDDfy();
        }

        [Test]
        public void UsingTransactionPerScriptScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldRollbackFailedScriptAndStopExecution())
                .BDDfy();
        }

        [Test]
        public void UsingSingleTransactionScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldExecuteAllScriptsInASingleTransaction())
                .BDDfy();
        }

        [Test]
        public void UsingSingleTransactionScenarioSuccessScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldRollbackFailedScriptAndStopExecution())
                .BDDfy();
        }

        void UpgradeIsPerformedWithFirstOfTwoScriptsFails()
        {
            scripts = new[]
            {
                new SqlScript("Script0001.sql", "error"),
                new SqlScript("Script0002.sql", "print 'script2'")
            };
            upgradeEngineBuilder
                .WithScripts(scripts)
                .Build()
                .PerformUpgrade();
        }

        void ShouldStopExecution()
        {
            testConnection.GetCommandLog().ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldRollbackFailedScriptAndStopExecution()
        {
            testConnection.GetCommandLog().ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldExecuteAllScriptsInASingleTransaction()
        {
            testConnection.GetCommandLog().ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldHaveExecutedEachScriptInATransaction()
        {
            testConnection.GetCommandLog().ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldExecuteScriptsWithoutUsingATransaction()
        {
            testConnection.GetCommandLog().ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void DbUpSetupToUseSingleTransaction()
        {
            testConnection = new RecordingDbConnection(false);
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .WithTransaction();
        }

        void DbUpSetupToNotUseTransactions()
        {
            testConnection = new RecordingDbConnection(false);
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .WithoutTransaction();
        }

        void DbUpSetupToUseTransactionPerScript()
        {
            testConnection = new RecordingDbConnection(false);
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .WithTransactionPerScript();
        }

        void UpgradeIsPerformedExecutingTwoScripts()
        {
            scripts = new[]
            {
                new SqlScript("Script0001.sql", "print 'script1'"),
                new SqlScript("Script0002.sql", "print 'script2'")
            };
             var result = upgradeEngineBuilder
                .WithScripts(scripts)
                .Build()
                .PerformUpgrade();
        }
    }
}