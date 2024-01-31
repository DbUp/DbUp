using System.Threading.Tasks;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Tests.Common;
using DbUp.Tests.Common.RecordingDb;
using DbUp.Tests.TestInfrastructure;
using TestStack.BDDfy;
using VerifyXunit;
using Xunit;

namespace DbUp.Tests;

[UsesVerify]
public class TransactionScenarios
{
    readonly TestProvider testProvider = new();

    [Fact]
    public Task UsingNoTransactionsScenario()
    {
        this
            .Given(_ => _.GivenDbUpSetupToNotUseTransactions())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .BDDfy();

        return ShouldExecuteScriptsWithoutUsingATransaction(nameof(UsingNoTransactionsScenario));
    }

    [Fact]
    public Task UsingNoTransactionsScenarioScriptFails()
    {
        this
            .Given(_ => _.GivenDbUpSetupToNotUseTransactions())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .BDDfy();
        return ShouldStopExecution(nameof(UsingNoTransactionsScenarioScriptFails));
    }

    [Fact]
    public Task UsingTransactionPerScriptScenarioSuccess()
    {
        this
            .Given(_ => _.GivenDbUpSetupToUseTransactionPerScript())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .BDDfy();
        return ShouldHaveExecutedEachScriptInATransaction(nameof(UsingTransactionPerScriptScenarioSuccess));
    }

    [Fact]
    public Task UsingTransactionPerScriptScenarioScriptFails()
    {
        this
            .Given(_ => _.GivenDbUpSetupToUseTransactionPerScript())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .BDDfy();
        return ShouldRollbackFailedScriptAndStopExecution(nameof(UsingTransactionPerScriptScenarioScriptFails));
    }

    [Fact]
    public Task UsingSingleTransactionScenarioSuccess()
    {
        this
            .Given(_ => GivenDbUpSetupToUseSingleTransaction())
            .And(_ => _.GivenAScriptIsProvided("Script0001.sql", "print 'script1'"))
            .And(_ => _.GivenAScriptIsProvided("Script0002.sql", "print 'script2'"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .BDDfy();
        return ShouldExecuteAllScriptsInASingleTransaction(nameof(UsingSingleTransactionScenarioSuccess));
    }

    [Fact]
    public Task UsingSingleTransactionScenarioSuccessScriptFails()
    {
        this
            .Given(_ => _.GivenDbUpSetupToUseSingleTransaction())
            .And(_ => _.GivenAScriptIsProvided("Script0001.sql", "error"))
            .And(_ => _.GivenAScriptIsProvided("Script0002.sql", "print 'script2'"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .BDDfy();
        return ShouldRollbackFailedScriptAndStopExecution(nameof(UsingSingleTransactionScenarioSuccessScriptFails));
    }

    Task ShouldStopExecution(string testName)
    {
        return Verifier.Verify(testProvider.Log.Log, VerifyHelper.GetVerifySettings());
    }

    Task ShouldRollbackFailedScriptAndStopExecution(string testName)
    {
        return Verifier.Verify(testProvider.Log.Log, VerifyHelper.GetVerifySettings());
    }

    Task ShouldExecuteAllScriptsInASingleTransaction(string testName)
    {
        return Verifier.Verify(testProvider.Log.Log, VerifyHelper.GetVerifySettings());
    }

    Task ShouldHaveExecutedEachScriptInATransaction(string testName)
    {
        return Verifier.Verify(testProvider.Log.Log, VerifyHelper.GetVerifySettings());
    }

    Task ShouldExecuteScriptsWithoutUsingATransaction(string testName)
    {
        return Verifier.Verify(testProvider.Log.Log, VerifyHelper.GetVerifySettings());
    }

    void GivenDbUpSetupToUseSingleTransaction()
        => testProvider.Builder.WithTransaction();

    void GivenTwoValidScriptsAreProvided()
        => testProvider.Builder.WithScripts(
            new SqlScript("Script0001.sql", "print 'script1'"),
            new SqlScript("Script0002.sql", "print 'script2'")
        );

    void GivenAScriptIsProvided(string name, string contents)
        => testProvider.Builder.WithScript(name, contents);

    void GivenDbUpSetupToNotUseTransactions()
        => testProvider.Builder.WithoutTransaction();

    void GivenDbUpSetupToUseTransactionPerScript()
        => testProvider.Builder.WithTransactionPerScript();

    void WhenUpgradeIsPerformed()
        => testProvider.Builder.Build().PerformUpgrade();
}
