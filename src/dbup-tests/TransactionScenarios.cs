using System.Threading.Tasks;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Tests.Common;
using DbUp.Tests.Common.RecordingDb;
using DbUp.Tests.TestInfrastructure;
using TestStack.BDDfy;
using VerifyXunit;
using Xunit;

namespace DbUp.Tests
{
    [UsesVerify]
    public class TransactionScenarios
    {
        UpgradeEngineBuilder upgradeEngineBuilder;
        RecordingDbConnection testConnection;
        SqlScript[] scripts;
        readonly CaptureLogsLogger logger = new();

        [Fact]
        public Task UsingNoTransactionsScenario()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .BDDfy();

            return ShouldExecuteScriptsWithoutUsingATransaction(nameof(UsingNoTransactionsScenario));
        }

        [Fact]
        public Task UsingNoTransactionsScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToNotUseTransactions())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .BDDfy();
            return ShouldStopExecution(nameof(UsingNoTransactionsScenarioScriptFails));
        }

        [Fact]
        public Task UsingTransactionPerScriptScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .BDDfy();
            return ShouldHaveExecutedEachScriptInATransaction(nameof(UsingTransactionPerScriptScenarioSuccess));
        }

        [Fact]
        public Task UsingTransactionPerScriptScenarioScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseTransactionPerScript())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .BDDfy();
            return ShouldRollbackFailedScriptAndStopExecution(nameof(UsingTransactionPerScriptScenarioScriptFails));
        }

        [Fact]
        public Task UsingSingleTransactionScenarioSuccess()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedExecutingTwoScripts())
                .BDDfy();
            return ShouldExecuteAllScriptsInASingleTransaction(nameof(UsingSingleTransactionScenarioSuccess));
        }

        [Fact]
        public Task UsingSingleTransactionScenarioSuccessScriptFails()
        {
            this
                .Given(_ => DbUpSetupToUseSingleTransaction())
                .When(_ => UpgradeIsPerformedWithFirstOfTwoScriptsFails())
                .BDDfy();
            return ShouldRollbackFailedScriptAndStopExecution(nameof(UsingSingleTransactionScenarioSuccessScriptFails));
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

        Task ShouldStopExecution(string testName)
        {
            return Verifier.Verify(logger.Log, VerifyHelper.GetVerifySettings());
        }

        Task ShouldRollbackFailedScriptAndStopExecution(string testName)
        {
            return Verifier.Verify(logger.Log, VerifyHelper.GetVerifySettings());
        }

        Task ShouldExecuteAllScriptsInASingleTransaction(string testName)
        {
            return Verifier.Verify(logger.Log, VerifyHelper.GetVerifySettings());
        }

        Task ShouldHaveExecutedEachScriptInATransaction(string testName)
        {
            return Verifier.Verify(logger.Log, VerifyHelper.GetVerifySettings());
        }

        Task ShouldExecuteScriptsWithoutUsingATransaction(string testName)
        {
            return Verifier.Verify(logger.Log, VerifyHelper.GetVerifySettings());
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
