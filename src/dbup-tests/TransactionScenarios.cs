using Assent;
using Assent.Namers;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Tests.TestInfrastructure;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests
{
    public class TransactionScenarios
    {
        UpgradeEngineBuilder upgradeEngineBuilder;
        RecordingDbConnection testConnection;
        SqlScript[] scripts;
        readonly CaptureLogsLogger logger;
        readonly Configuration assentConfig = new Configuration()
            .UsingNamer(new SubdirectoryNamer("ApprovalFiles"))
            .UsingSanitiser(Scrubbers.ScrubDates);

        public TransactionScenarios()
        {
            logger = new CaptureLogsLogger();

            // Automatically approve the change, make sure to check the result before committing 
            // assentConfig = assentConfig.UsingReporter((received, approved) => File.Copy(received, approved, true));
        }

        [Fact]
        public void UsingNoTransactionsScenario()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldExecuteScriptsWithoutUsingATransaction(nameof(UsingNoTransactionsScenario)))
                .BDDfy();
        }

        [Fact]
        public void UsingNoTransactionsScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldStopExecution(nameof(UsingNoTransactionsScenarioScriptFails)))
                .BDDfy();
        }

        [Fact]
        public void UsingTransactionPerScriptScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldHaveExecutedEachScriptInATransaction(nameof(UsingTransactionPerScriptScenarioSuccess)))
                .BDDfy();
        }

        [Fact]
        public void UsingTransactionPerScriptScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldRollbackFailedScriptAndStopExecution(nameof(UsingTransactionPerScriptScenarioScriptFails)))
                .BDDfy();
        }

        [Fact]
        public void UsingSingleTransactionScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .Then(_ => ShouldExecuteAllScriptsInASingleTransaction(nameof(UsingSingleTransactionScenarioSuccess)))
                .BDDfy();
        }

        [Fact]
        public void UsingSingleTransactionScenarioSuccessScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .Then(_ => ShouldRollbackFailedScriptAndStopExecution(nameof(UsingSingleTransactionScenarioSuccessScriptFails)))
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

        void ShouldStopExecution(string testName)
        {
            this.Assent(logger.Log, assentConfig, testName);
        }

        void ShouldRollbackFailedScriptAndStopExecution(string testName)
        {
            this.Assent(logger.Log, assentConfig, testName);
        }

        void ShouldExecuteAllScriptsInASingleTransaction(string testName)
        {
            this.Assent(logger.Log, assentConfig, testName);
        }

        void ShouldHaveExecutedEachScriptInATransaction(string testName)
        {
            this.Assent(logger.Log, assentConfig, testName);
        }

        void ShouldExecuteScriptsWithoutUsingATransaction(string testName)
        {
            this.Assent(logger.Log, assentConfig, testName);
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