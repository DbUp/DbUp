using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Tests.Common.RecordingDb;
using Shouldly;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using VerifyXunit;

namespace DbUp.Tests.Common;

[UsesVerify]
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
    public Task VerifyBasicSupport()
    {
        this
            .Given(_ => DeployTo())
            .And(_ => TargetDatabaseIsEmpty())
            .And(_ => SingleScriptExists())
            .When(_ => UpgradeIsPerformed())
            .Then(_ => UpgradeIsSuccessful())
            .BDDfy();
        return CommandLogReflectsScript(nameof(VerifyBasicSupport));
    }

    [BddfyFact]
    public Task VerifyVariableSubstitutions()
    {
        this
            .Given(_ => DeployTo())
            .And(_ => TargetDatabaseIsEmpty())
            .And(_ => SingleScriptWithVariableUsageExists())
            .And(_ => VariableSubstitutionIsSetup())
            .When(_ => UpgradeIsPerformed())
            .Then(_ => UpgradeIsSuccessful())
            .BDDfy();

        return CommandLogReflectsScript(nameof(VerifyVariableSubstitutions));
    }

    [BddfyFact]
    public Task VerifyJournalCreationIfNameChanged()
    {
        this
            .Given(_ => DeployTo())
            .And(_ => TargetDatabaseIsEmpty())
            .And(_ => JournalTableNameIsCustomised())
            .And(_ => SingleScriptExists())
            .When(_ => UpgradeIsPerformed())
            .Then(_ => UpgradeIsSuccessful())
            .BDDfy();
        return CommandLogReflectsScript(nameof(VerifyJournalCreationIfNameChanged));
    }


    void VariableSubstitutionIsSetup()
    {
        upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
    }

    void JournalTableNameIsCustomised()
    {
        upgradeEngineBuilder = AddCustomNamedJournalToBuilder(upgradeEngineBuilder!, "test", "TestSchemaVersions");
    }

    Task CommandLogReflectsScript(string testName)
    {
        return Verifier.Verify(logger.Log, VerifyHelper.GetVerifySettings(), sourceFile: parentFilePath!);
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
