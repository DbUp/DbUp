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
using DbUp.Redshift;
#endif

namespace DbUp.Tests
{
    public class DatabaseSupportTests
    {
        IConnectionFactory testConnectionFactory;
        UpgradeEngineBuilder upgradeEngineBuilder;
        List<SqlScript> scripts;
        RecordingDbConnection recordingConnection;
        DatabaseUpgradeResult result;
        Func<UpgradeEngineBuilder, string, string, UpgradeEngineBuilder> addCustomNamedJournalToBuilder;
        CaptureLogsLogger logger;

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

        ExampleTable DatabaseExampleTable => new ExampleTable("Deploy to")
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
                    new ExampleAction("Redshift", Deploy(to => to.RedshiftDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new RedshiftTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("SqlCe", Deploy(to => to.SqlCeDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("MySql", Deploy(to => to.MySqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new MySqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; }))                    
#endif
                };

        void VariableSubstitutionIsSetup()
        {
            upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
        }

        void JournalTableNameIsCustomised()
        {
            upgradeEngineBuilder = addCustomNamedJournalToBuilder(upgradeEngineBuilder, "test", "TestSchemaVersions");
        }

        void CommandLogReflectsScript(ExampleAction target, string testName)
        {
            var configuration = new Configuration()
                .UsingSanitiser(Scrubbers.ScrubDates)
                .UsingNamer(new Namer(target, testName));

            // Automatically approve the change, make sure to check the result before committing 
            // configuration = configuration.UsingReporter((received, approved) => File.Copy(received, approved, true));

            this.Assent(logger.Log, configuration);
        }

        void UpgradeIsSuccessful()
        {
            result.Successful.ShouldBe(true);
        }

        void UpgradeIsPerformed()
        {
            result = upgradeEngineBuilder.Build().PerformUpgrade();
        }

        void SingleScriptExists()
        {
            scripts.Add(new SqlScript("Script0001.sql", "script1contents"));
        }

        void SingleScriptWithVariableUsageExists()
        {
            scripts.Add(new SqlScript("Script0001.sql", "print $TestVariable$"));
        }

        void TargetDatabaseIsEmpty()
        {
        }

        Action Deploy(Func<SupportedDatabases, UpgradeEngineBuilder> deployTo, Func<UpgradeEngineBuilder, string, string, UpgradeEngineBuilder> addCustomNamedJournal)
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

        class Namer : INamer
        {
            readonly ExampleAction target;
            readonly string testName;

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
