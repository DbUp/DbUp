using System;
using System.Collections.Generic;
using System.IO;
using Assent;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Oracle;
using DbUp.SQLite;
using DbUp.SqlServer;
using DbUp.Tests.TestInfrastructure;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

#if !NETCORE
using DbUp.Firebird;
using DbUp.MySql;
using DbUp.Postgresql;
#endif

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
        private CaptureLogsLogger logger;

        [Fact]
        public void VerifyBasicSupport()
        {
            ExampleAction deployTo = null;
            this
                .Given(() => deployTo)
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => SingleScriptExists())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .And(_ => CommandLogReflectsScript(deployTo, nameof(VerifyBasicSupport)), "Command log matches expected steps")
                .WithExamples(DatabaseExampleTable)
                .BDDfy();
        }

        [Fact]
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
                .And(_ => CommandLogReflectsScript(deployTo, nameof(VerifyVariableSubstitutions)), "Variables substituted correctly in command log")
                .WithExamples(DatabaseExampleTable)
                .BDDfy();
        }

        [Fact]
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
                .And(_ => CommandLogReflectsScript(deployTo, nameof(VerifyJournalCreationIfNameChanged)), "Command log matches expected steps")
                .WithExamples(DatabaseExampleTable)
                .BDDfy();
        }

        private ExampleTable DatabaseExampleTable => new ExampleTable("Deploy to")
                {
                    new ExampleAction("Sql Server", Deploy(to => to.SqlDatabase(string.Empty), (builder, schema, tableName) =>
                    {
                        builder.Configure(c => c.Journal = new SqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, tableName));
                        return builder;
                    })),
                    new ExampleAction("SQLite", Deploy(to => to.SQLiteDatabase(string.Empty), (builder, schema, tableName) =>
                    {
                        builder.Configure(c => c.Journal = new SQLiteTableJournal(() => c.ConnectionManager, () => c.Log, tableName));
                        return builder;
                    })),
                    new ExampleAction("Oracle", Deploy(to => to.OracleDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new OracleTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),

#if !NETCORE
                    new ExampleAction("Firebird", Deploy(to => to.FirebirdDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new FirebirdTableJournal(()=>c.ConnectionManager, ()=>c.Log, tableName)); return builder; })),
                    new ExampleAction("PostgreSQL", Deploy(to => to.PostgresqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new PostgresqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("SqlCe", Deploy(to => to.SqlCeDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("MySql", Deploy(to => to.MySqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new MySqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; }))                    
#endif
                };

        private void VariableSubstitutionIsSetup()
        {
            upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
        }

        private void JournalTableNameIsCustomised()
        {
            upgradeEngineBuilder = addCustomNamedJournalToBuilder(upgradeEngineBuilder, "test", "TestSchemaVersions");
        }

        private void CommandLogReflectsScript(ExampleAction target, string testName)
        {
            var configuration = new Configuration()
                .UsingSanitiser(Scrubbers.ScrubDates)
                .UsingNamer(new Namer(target, testName));

            // Automatically approve the change, make sure to check the result before committing 
            // configuration = configuration.UsingReporter((received, approved) => File.Copy(received, approved, true));

            this.Assent(logger.Log, configuration);
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
                logger = new CaptureLogsLogger();
                recordingConnection = new RecordingDbConnection(logger, "SchemaVersions");
                testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);
                upgradeEngineBuilder = deployTo(DeployChanges.To)
                    .WithScripts(scripts)
                    .OverrideConnectionFactory(testConnectionFactory)
                    .LogTo(logger);

                addCustomNamedJournalToBuilder = addCustomNamedJournal;
            };
        }

        private class Namer : INamer
        {
            private readonly ExampleAction target;
            private readonly string testName;

            public Namer(ExampleAction target, string testName)
            {
                this.target = target;
                this.testName = testName;
            }

            public string GetName(TestMetadata metadata)
            {
                var targetName = target.ToString().Replace(" ", "");
                var dir = Path.GetDirectoryName(metadata.FilePath);
                var filename = $"{metadata.TestFixture.GetType().Name}.{testName}.{targetName}";

                return Path.Combine(dir, "ApprovalFiles", filename);
            }
        }
    }
}