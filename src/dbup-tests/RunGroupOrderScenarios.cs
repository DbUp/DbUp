using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;
using DbUp.Tests.TestInfrastructure;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests;

[Story(
    AsA = "As a DbUp User",
    IWant = "I want to DbUp to upgrade my database to the latest version using my own sort order",
    SoThat = "So that my application's database is up to date")]
public class RunGroupOrderScenarios
{
    DatabaseUpgradeResult result;
    readonly TestProvider testProvider;

    public RunGroupOrderScenarios()
    {
        testProvider = new TestProvider();

        testProvider.Builder.WithScripts(
            new SqlScript("ZZZScript1.sql", "create table Foo (Id int identity)", new SqlScriptOptions {ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder}),
            new SqlScript("ZZZScript2.sql", "alter table Foo add column Name varchar(255)", new SqlScriptOptions {ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder}),
            new SqlScript("AAAScript3.sql", "insert into Foo (Name) values ('test')", new SqlScriptOptions {ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder + 1})
        );
    }

    [Fact]
    public void UpgradingAnOutOfDateDatabase()
    {
        this.Given(t => t.GivenAnOutOfDateDatabase())
            .When(t => t.WhenDatabaseIsUpgraded())
            .Then(t => t.ThenShouldHaveSuccessfulResult())
            .And(t => t.AndShouldHaveRunAllScriptsInOrder())
            .And(t => t.AndShouldLogInformation())
            .BDDfy();
    }

    void AndShouldLogInformation()
    {
        testProvider.Log.InfoMessages.ShouldContain("Beginning database upgrade");
        testProvider.Log.InfoMessages.ShouldContain("Upgrade successful");
    }

    void AndShouldHaveRunAllScriptsInOrder()
    {
        // Check both results and journal
        result.Scripts
            .Select(s => s.Name)
            .ShouldBe(new[] {"ZZZScript1.sql", "ZZZScript2.sql", "AAAScript3.sql"});
    }

    void ThenShouldHaveSuccessfulResult()
    {
        result.Successful.ShouldBeTrue();
    }

    void GivenAnOutOfDateDatabase()
    {
    }

    void WhenDatabaseIsUpgraded()
    {
        var upgradeEngine = testProvider.Builder.Build();
        result = upgradeEngine.PerformUpgrade();
    }

    public class TestScriptProvider : IScriptProvider
    {
        readonly List<SqlScript> sqlScripts;

        public TestScriptProvider(List<SqlScript> sqlScripts)
        {
            this.sqlScripts = sqlScripts;
        }

        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return sqlScripts;
        }
    }
}
