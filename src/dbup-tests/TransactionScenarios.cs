#if !NETCORE
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Tests.TestInfrastructure;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests
{
    public class TransactionScenarios
    {
        UpgradeEngineBuilder upgradeEngineBuilder;
        RecordingDbConnection testConnection;
        SqlScript[] scripts;
        CaptureLogsLogger logger;

        public TransactionScenarios()
        {
            logger = new CaptureLogsLogger();
        }

        [Fact]
        public void UsingNoTransactionsScenario()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldExecuteScriptsWithoutUsingATransaction())
                .BDDfy();
        }

        [Fact]
        public void UsingNoTransactionsScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldStopExecution())
                .BDDfy();
        }

        [Fact]
        public void UsingTransactionPerScriptScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldHaveExecutedEachScriptInATransaction())
                .BDDfy();
        }

        [Fact]
        public void UsingTransactionPerScriptScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldRollbackFailedScriptAndStopExecution())
                .BDDfy();
        }

        [Fact]
        public void UsingSingleTransactionScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldExecuteAllScriptsInASingleTransaction())
                .BDDfy();
        }

        [Fact]
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
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<FactAttribute>();
                b.SubFolder("ApprovalFiles");
            });
        }

        void ShouldRollbackFailedScriptAndStopExecution()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<FactAttribute>();
                b.SubFolder("ApprovalFiles");
            });
        }

        void ShouldExecuteAllScriptsInASingleTransaction()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<FactAttribute>();
                b.SubFolder("ApprovalFiles");
            });
        }

        void ShouldHaveExecutedEachScriptInATransaction()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<FactAttribute>();
                b.SubFolder("ApprovalFiles");
            });
        }

        void ShouldExecuteScriptsWithoutUsingATransaction()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<FactAttribute>();
                b.SubFolder("ApprovalFiles");
            });
        }

        void DbUpSetupToUseSingleTransaction()
        {
            testConnection = new RecordingDbConnection(logger, "SchemaVersions");
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .JournalToSqlTable("dbo", "SchemaVersions")
                .WithTransaction();
        }

        void DbUpSetupToNotUseTransactions()
        {
            testConnection = new RecordingDbConnection(logger, "SchemaVersions");
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .JournalToSqlTable("dbo", "SchemaVersions")
                .WithoutTransaction();
        }

        void DbUpSetupToUseTransactionPerScript()
        {
            testConnection = new RecordingDbConnection(logger, "SchemaVersions");
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .JournalToSqlTable("dbo", "SchemaVersions")
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
                .LogTo(logger)
                .Build()
                .PerformUpgrade();
        }
    }
}
#endif