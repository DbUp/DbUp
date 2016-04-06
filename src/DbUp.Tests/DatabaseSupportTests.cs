using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Namers;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support.Firebird;
using DbUp.Support.MySql;
using DbUp.Support.Postgresql;
using DbUp.Support.SQLite;
using DbUp.Support.SqlServer;
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
        private Func<UpgradeEngineBuilder, string, string, UpgradeEngineBuilder> addCustomNamedJournalToBuilder;

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

        [Test]
        public void VerifyJournalCreationIfNameChanged()
        {
            ExampleAction deployTo = null;
            this
                .Given(() => deployTo)
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => JournalTableNameIsCustomised())
                .And(_ => SingleScriptExists())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .And(_ => CommandLogReflectsScript(deployTo), "Command log matches expected steps")
                .WithExamples(DatabaseExampleTable)
                .BDDfy();
        }

        private ExampleTable DatabaseExampleTable
        {
            get
            {
                return new ExampleTable("Deploy to")
                {
                    { new ExampleAction("Sql Server", Deploy(to => to.SqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })) },
                    { new ExampleAction("Firebird", Deploy(to => to.FirebirdDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new FirebirdTableJournal(()=>c.ConnectionManager, ()=>c.Log, tableName)); return builder; })) },
                    { new ExampleAction("PostgreSQL", Deploy(to => to.PostgresqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new PostgresqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })) },
                    { new ExampleAction("SQLite", Deploy(to => to.SQLiteDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SQLiteTableJournal(()=>c.ConnectionManager, ()=>c.Log, tableName)); return builder; })) },
                    { new ExampleAction("SqlCe", Deploy(to => to.SqlCeDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })) },
                    { new ExampleAction("MySql", Deploy(to => to.MySqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new MySqlITableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })) }
                };
            }
        }

        private void VariableSubstitutionIsSetup()
        {
            upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
        }

        private void JournalTableNameIsCustomised()
        {
            upgradeEngineBuilder = addCustomNamedJournalToBuilder(upgradeEngineBuilder, "test", "TestSchemaVersions");
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

        private Action Deploy(Func<SupportedDatabases, UpgradeEngineBuilder> deployTo, Func<UpgradeEngineBuilder, string, string, UpgradeEngineBuilder> addCustomNamedJournal)
        {
            return () =>
            {
                scripts = new List<SqlScript>();
                recordingConnection = new RecordingDbConnection(false);
                testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);
                upgradeEngineBuilder = deployTo(DeployChanges.To)
                    .WithScripts(scripts)
                    .OverrideConnectionFactory(testConnectionFactory);

                addCustomNamedJournalToBuilder = addCustomNamedJournal;
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