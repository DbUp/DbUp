using System;
using System.Data;
using Assent;
using Assent.Namers;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Tests.Common;
using DbUp.Tests.TestInfrastructure;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests;

public class TransactionScenarios
{
    readonly TestProvider testProvider = new();

    readonly Configuration assentConfig = new Configuration()
        .UsingNamer(new SubdirectoryNamer("ApprovalFiles"))
        .UsingSanitiser(Scrubbers.ScrubDates);

    [Fact]
    public void UsingNoTransactionsScenario()
    {
        this
            .Given(_ => _.GivenDbUpSetupToNotUseTransactions())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenShouldMatchApprovalFile(nameof(UsingNoTransactionsScenario)))
            .BDDfy();
    }

    [Fact]
    public void UsingNoTransactionsScenarioScriptFails()
    {
        this
            .Given(_ => _.GivenDbUpSetupToNotUseTransactions())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenShouldMatchApprovalFile(nameof(UsingNoTransactionsScenarioScriptFails)))
            .BDDfy();
    }

    [Fact]
    public void UsingTransactionPerScriptScenarioSuccess()
    {
        this
            .Given(_ => _.GivenDbUpSetupToUseTransactionPerScript())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .And(_ => _.GivenACodeScriptIsProvided(new ScriptWithChangeInProvideScriptMethod(testProvider.Log)))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenShouldMatchApprovalFile(nameof(UsingTransactionPerScriptScenarioSuccess)))
            .BDDfy();
    }
    

    [Fact]
    public void UsingTransactionPerScriptScenarioScriptFails()
    {
        this
            .Given(_ => _.GivenDbUpSetupToUseTransactionPerScript())
            .And(_ => _.GivenTwoValidScriptsAreProvided())
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenShouldMatchApprovalFile(nameof(UsingTransactionPerScriptScenarioScriptFails)))
            .BDDfy();
    }

    [Fact]
    public void UsingSingleTransactionScenarioSuccess()
    {
        this
            .Given(_ => GivenDbUpSetupToUseSingleTransaction())
            .And(_ => _.GivenAScriptIsProvided("Script0001.sql", "print 'script1'"))
            .And(_ => _.GivenAScriptIsProvided("Script0002.sql", "print 'script2'"))
            .And(_ => _.GivenACodeScriptIsProvided(new ScriptWithChangeInProvideScriptMethod(testProvider.Log)))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenShouldMatchApprovalFile(nameof(UsingSingleTransactionScenarioSuccess)))
            .BDDfy();
    }

    [Fact]
    public void UsingSingleTransactionScenarioSuccessScriptFails()
    {
        this
            .Given(_ => _.GivenDbUpSetupToUseSingleTransaction())
            .And(_ => _.GivenAScriptIsProvided("Script0001.sql", "error"))
            .And(_ => _.GivenAScriptIsProvided("Script0002.sql", "print 'script2'"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenShouldMatchApprovalFile(nameof(UsingSingleTransactionScenarioSuccessScriptFails)))
            .BDDfy();
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
    
    void GivenACodeScriptIsProvided(IScript script)
        => testProvider.Builder.WithScript(script.GetType().Name, script);

    void GivenDbUpSetupToNotUseTransactions()
        => testProvider.Builder.WithoutTransaction();

    void GivenDbUpSetupToUseTransactionPerScript()
        => testProvider.Builder.WithTransactionPerScript();

    void WhenUpgradeIsPerformed()
        => testProvider.Builder.Build().PerformUpgrade();

    void ThenShouldMatchApprovalFile(string testName)
    {
        this.Assent(testProvider.Log.Log, assentConfig, testName);
    }

    public class ScriptWithChangeInProvideScriptMethod(IUpgradeLog upgradeLog) : IScript
    {
        public string ProvideScript(Func<IDbCommand> commandFactory)
        {
            upgradeLog.LogInformation("Running script in ProvideScript method");
            var dbcommand = commandFactory(); // DbCommand is creat with committed transaction
            dbcommand.CommandText = "select 1";
            dbcommand.ExecuteScalar();
            
            return "";
        }
    }
}
