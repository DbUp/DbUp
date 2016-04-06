﻿using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
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
        CaptureLogsLogger logger;

        [SetUp]
        public void CaptureLogs()
        {
            logger = new CaptureLogsLogger();
        }

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
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldRollbackFailedScriptAndStopExecution()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldExecuteAllScriptsInASingleTransaction()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldHaveExecutedEachScriptInATransaction()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void ShouldExecuteScriptsWithoutUsingATransaction()
        {
            logger.Log.ShouldMatchApproved(b =>
            {
                b.WithScrubber(Scrubbers.ScrubDates);
                b.LocateTestMethodUsingAttribute<TestAttribute>();
            });
        }

        void DbUpSetupToUseSingleTransaction()
        {
            testConnection = new RecordingDbConnection(logger, false, "SchemaVersions");
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .JournalToSqlTable("dbo", "SchemaVersions")
                .WithTransaction();
        }

        void DbUpSetupToNotUseTransactions()
        {
            testConnection = new RecordingDbConnection(logger, false, "SchemaVersions");
            upgradeEngineBuilder = DeployChanges.To
                .TestDatabase(testConnection)
                .JournalToSqlTable("dbo", "SchemaVersions")
                .WithoutTransaction();
        }

        void DbUpSetupToUseTransactionPerScript()
        {
            testConnection = new RecordingDbConnection(logger, false, "SchemaVersions");
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