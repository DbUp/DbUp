﻿using System;
using System.Collections.Generic;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Firebird;
using DbUp.MySql;
using DbUp.Postgresql;
using DbUp.SqlServer;
using DbUp.SQLite;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;

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

        ExampleTable DatabaseExampleTable
        {
            get
            {
                return new ExampleTable("Deploy to")
                {
                    new ExampleAction("Sql Server", Deploy(to => to.SqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("Firebird", Deploy(to => to.FirebirdDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new FirebirdTableJournal(()=>c.ConnectionManager, ()=>c.Log, tableName)); return builder; })),
                    new ExampleAction("PostgreSQL", Deploy(to => to.PostgresqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new PostgresqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("SQLite", Deploy(to => to.SQLiteDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SQLiteTableJournal(()=>c.ConnectionManager, ()=>c.Log, tableName)); return builder; })),
                    new ExampleAction("SqlCe", Deploy(to => to.SqlCeDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; })),
                    new ExampleAction("MySql", Deploy(to => to.MySqlDatabase(string.Empty), (builder, schema, tableName) => { builder.Configure(c => c.Journal = new MySqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, tableName)); return builder; }))
                };
            }
        }

        void VariableSubstitutionIsSetup()
        {
            upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
        }

        void JournalTableNameIsCustomised()
        {
            upgradeEngineBuilder = addCustomNamedJournalToBuilder(upgradeEngineBuilder, "test", "TestSchemaVersions");
        }

        void CommandLogReflectsScript(ExampleAction target)
        {
            logger.Log
                .ShouldMatchApproved(b =>
                {
                    b.LocateTestMethodUsingAttribute<TestAttribute>();
                    b.WithScrubber(Scrubbers.ScrubDates);
                    b.WithDescriminator(target.ToString().Replace(" ", string.Empty));
                });
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
                recordingConnection = new RecordingDbConnection(logger, false, "SchemaVersions");
                testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);
                upgradeEngineBuilder = deployTo(DeployChanges.To)
                    .WithScripts(scripts)
                    .OverrideConnectionFactory(testConnectionFactory)
                    .LogTo(logger);

                addCustomNamedJournalToBuilder = addCustomNamedJournal;
            };
        }
    }
}