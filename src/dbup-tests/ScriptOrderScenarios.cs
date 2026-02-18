using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Engine.Transactions;
using DbUp.Support;
using DbUp.Tests.TestInfrastructure;
using TestStack.BDDfy;

namespace DbUp.Tests;

[Story(
    AsA = "As a DbUp User",
    IWant = "I want to DbUp to upgrade my database to the latest version using my own sort order",
    SoThat = "So that my application's database is up to date")]
public class ScriptOrderScenarios
{
    DatabaseUpgradeResult result;
    readonly TestProvider testProvider;

    public ScriptOrderScenarios()
    {
        testProvider = new TestProvider();

        testProvider.Builder.WithScripts(
            new SqlScript("ZZZScript1.sql", "create table Foo (Id int identity)", new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder }),
            new SqlScript("ZZZScript2.sql", "alter table Foo add column Name varchar(255)", new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder }),
            new SqlScript("AAAScript3.sql", "insert into Foo (Name) values ('test')", new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder + 1 })
        );
    }

    [Fact]
    public void UpgradingAnOutOfDateDatabaseWithDefaultSort()
    {
        this.Given(t => t.GivenAnOutOfDateDatabase())
            .When(t => t.WhenDatabaseIsUpgradedWithDefaultSort())
            .Then(t => t.ThenShouldHaveSuccessfulResult())
            .And(t => t.AndShouldHaveRunAllScriptsInDefaultOrder())
            .And(t => t.AndShouldLogInformation())
            .BDDfy();
    }

    [Fact]
    public void UpgradingAnOutOfDateDatabaseWithReverseNameSort()
    {
        this.Given(t => t.GivenAnOutOfDateDatabase())
            .When(t => t.WhenDatabaseIsUpgradedWithReverseNameSort())
            .Then(t => t.ThenShouldHaveSuccessfulResult())
            .And(t => t.AndShouldHaveRunAllScriptsInReverseNameOrder())
            .And(t => t.AndShouldLogInformation())
            .BDDfy();
    }

    void AndShouldLogInformation()
    {
        testProvider.Log.InfoMessages.ShouldContain("Beginning database upgrade");
        testProvider.Log.InfoMessages.ShouldContain("Upgrade successful");
    }

    void AndShouldHaveRunAllScriptsInDefaultOrder()
    {
        // Check both results and journal
        result.Scripts
            .Select(s => s.Name)
            .ShouldBe(new[] { "ZZZScript1.sql", "ZZZScript2.sql", "AAAScript3.sql" });
    }

    void AndShouldHaveRunAllScriptsInReverseNameOrder()
    {
        // Check both results and journal
        result.Scripts
            .Select(s => s.Name)
            .ShouldBe(new[] { "ZZZScript2.sql", "ZZZScript1.sql", "AAAScript3.sql" });
    }

    void ThenShouldHaveSuccessfulResult()
    {
        result.Successful.ShouldBeTrue();
    }

    void GivenAnOutOfDateDatabase()
    {
    }

    void WhenDatabaseIsUpgradedWithDefaultSort()
    {
        var upgradeEngine = testProvider.Builder.Build();
        result = upgradeEngine.PerformUpgrade();
    }

    void WhenDatabaseIsUpgradedWithReverseNameSort()
    {
        var upgradeEngine = testProvider
            .Builder
            .WithScriptSorter(scripts => scripts.OrderByDescending(s => s.Name, new ScriptNameComparer(StringComparer.Ordinal)))
            .Build();
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
