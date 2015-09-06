using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Namers;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;

namespace DbUp.Tests
{
    public class DatabaseSupportTests
    {
        private IConnectionFactory testConnectionFactory;
        private UpgradeEngineBuilder upgradeEngineBuilder;
        private List<SqlScript> scripts;
        private RecordingDbConnection recordingConnection;
        private DatabaseUpgradeResult result;

        [Test]
        public void VerifyBasicSupport()
        {
            ExampleAction deployTo = null;
            this
                .Given(() => deployTo)
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => SingleScriptExists())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .And(_ => CommandLogReflectsScript(deployTo), "Command log matches expected steps")
                .WithExamples(DatabaseExampleTable)
                .BDDfy();
        }

        [Test]
        public void VerifyVariableSubstitutions()
        {
            ExampleAction deployTo = null;
            this
                .Given(() => deployTo)
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => SingleScriptWithVariableUsageExists())
                .And(_ => VariableSubstitutionIsSetup())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .And(_ => CommandLogReflectsScript(deployTo), "Variables substituted correctly in command log")
                .WithExamples(DatabaseExampleTable)
                .BDDfy();
        }

        private ExampleTable DatabaseExampleTable
        {
            get
            {
                return new ExampleTable("Deploy to")
                {
                    { new ExampleAction("Sql Server", Deploy(to => to.SqlDatabase(string.Empty))) },
                    { new ExampleAction("Firebird", Deploy(to => to.FirebirdDatabase(string.Empty))) },
                    { new ExampleAction("PostgreSQL", Deploy(to => to.PostgresqlDatabase(string.Empty))) },
                    { new ExampleAction("SQLite", Deploy(to => to.SQLiteDatabase(string.Empty))) },
                    { new ExampleAction("SqlCe", Deploy(to => to.SqlCeDatabase(string.Empty))) },
                    { new ExampleAction("MySql", Deploy(to => to.MySqlDatabase(string.Empty))) }
                };
            }
        }

        private void VariableSubstitutionIsSetup()
        {
            upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
        }

        private void CommandLogReflectsScript(ExampleAction target)
        {
            Approvals.Verify(
                new ApprovalTextWriter(Scrubbers.ScrubDates(recordingConnection.GetCommandLog())),
                new CustomUnitTestFrameworkNamer(target.ToString().Replace(" ", string.Empty)),
                Approvals.GetReporter());
        }

        private void UpgradeIsSuccessful()
        {
            result.Successful.ShouldBe(true);
        }

        private void UpgradeIsPerformed()
        {
            result = upgradeEngineBuilder.Build().PerformUpgrade();
        }

        private void SingleScriptExists()
        {
            scripts.Add(new SqlScript("Script0001.sql", "script1contents"));
        }

        private void SingleScriptWithVariableUsageExists()
        {
            scripts.Add(new SqlScript("Script0001.sql", "print $TestVariable$"));
        }

        private void TargetDatabaseIsEmpty()
        {
        }

        private Action Deploy(Func<SupportedDatabases, UpgradeEngineBuilder> deployTo)
        {
            return () =>
            {
                scripts = new List<SqlScript>();
                recordingConnection = new RecordingDbConnection(false);
                testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);
                upgradeEngineBuilder = deployTo(DeployChanges.To)
                    .WithScripts(scripts)
                    .OverrideConnectionFactory(testConnectionFactory);
            };
        }
    }

    internal class CustomUnitTestFrameworkNamer : UnitTestFrameworkNamer
    {
        private readonly string additional;

        public CustomUnitTestFrameworkNamer(string additional)
        {
            this.additional = additional;
        }

        public override string Name { get { return base.Name + (string.IsNullOrEmpty(additional) ? null : ".") + additional; } }
    }
}