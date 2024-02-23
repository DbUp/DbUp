using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Assent;
using Assent.Namers;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Tests.Common.RecordingDb;
using Shouldly;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace DbUp.Tests.Common;

public abstract class DatabaseSupportTestsBase
{
    readonly string? parentFilePath;
    readonly IConnectionFactory testConnectionFactory;
    readonly List<SqlScript> scripts = new();
    readonly RecordingDbConnection recordingConnection;
    readonly CaptureLogsLogger logger = new();

    DatabaseUpgradeResult? result;
    UpgradeEngineBuilder? upgradeEngineBuilder;


    public DatabaseSupportTestsBase([CallerFilePath] string? parentFilePath = null)
    {
        this.parentFilePath = parentFilePath;
        testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);
        recordingConnection = new RecordingDbConnection(logger);
    }

    protected abstract UpgradeEngineBuilder DeployTo(SupportedDatabases to);

    protected abstract UpgradeEngineBuilder AddCustomNamedJournalToBuilder(
        UpgradeEngineBuilder builder,
        string schema,
        string tableName
    );

    [BddfyFact]
    public void VerifyBasicSupport()
    {
        this
            .Given(_ => DeployTo())
            .And(_ => TargetDatabaseIsEmpty())
            .And(_ => SingleScriptExists())
            .When(_ => UpgradeIsPerformed())
            .Then(_ => UpgradeIsSuccessful())
            .And(_ => CommandLogReflectsScript(nameof(VerifyBasicSupport)),
                "Command log matches expected steps")
            .BDDfy();
    }

    [BddfyFact]
    public void VerifyVariableSubstitutions()
    {
        this
            .Given(_ => DeployTo())
            .And(_ => TargetDatabaseIsEmpty())
            .And(_ => SingleScriptWithVariableUsageExists())
            .And(_ => VariableSubstitutionIsSetup())
            .When(_ => UpgradeIsPerformed())
            .Then(_ => UpgradeIsSuccessful())
            .And(_ => CommandLogReflectsScript(nameof(VerifyVariableSubstitutions)),
                "Variables substituted correctly in command log")
            .BDDfy();
    }

    [BddfyFact]
    public void VerifyJournalCreationIfNameChanged()
    {
        this
            .Given(_ => DeployTo())
            .And(_ => TargetDatabaseIsEmpty())
            .And(_ => JournalTableNameIsCustomised())
            .And(_ => SingleScriptExists())
            .When(_ => UpgradeIsPerformed())
            .Then(_ => UpgradeIsSuccessful())
            .And(_ => CommandLogReflectsScript(nameof(VerifyJournalCreationIfNameChanged)),
                "Command log matches expected steps")
            .BDDfy();
    }


    void VariableSubstitutionIsSetup()
    {
        upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
    }

    void JournalTableNameIsCustomised()
    {
        upgradeEngineBuilder = AddCustomNamedJournalToBuilder(upgradeEngineBuilder!, "test", "TestSchemaVersions");
    }


    void CommandLogReflectsScript(string testName)
    {
        var configuration = new Configuration()
            .UsingSanitiser(Scrubbers.ScrubDates)
            .UsingNamer(new SubdirectoryNamer("ApprovalFiles"));

        // Automatically approve the change, make sure to check the result before committing
        // configuration = configuration.UsingReporter((received, approved) => File.Copy(received, approved, true));

        this.Assent(logger.Log, configuration, testName, parentFilePath);
    }

    void UpgradeIsSuccessful()
    {
        result!.Successful.ShouldBe(true);
    }

    void UpgradeIsPerformed()
    {
        result = upgradeEngineBuilder!.Build().PerformUpgrade();
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

    void DeployTo()
    {
        upgradeEngineBuilder = DeployTo(DeployChanges.To)
            .WithScripts(scripts)
            .LogTo(logger);

        upgradeEngineBuilder.Configure(c => c.ConnectionManager = new TestConnectionManager(testConnectionFactory));
    }
}
