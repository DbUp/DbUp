using System;
using System.Data;
using System.Linq;
using DbUp.Engine;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests;

public class DeployChangesBuilderTests
{
    [Fact]
    public void can_use_variables_with_builder()
    {
        var testProvider = new TestProvider("Db");

        testProvider.Builder
            .WithScript("testscript", "$schema$Up $somevar$")
            .WithVariable("somevar", "is awesome");

        testProvider.Builder.Build().PerformUpgrade();

        testProvider.Connection.CommandsIssued.ShouldContain(c => c.CommandText == "[Db]Up is awesome");
    }

    [Fact]
    public void WithExecutionTimeout_Should_Set_CommandTimeout_Property_To_Given_Value()
    {
        var testProvider = new TestProvider();

        var upgradeEngine = testProvider.Builder
            .WithScript("testscript", "test")
            .WithExecutionTimeout(TimeSpan.FromSeconds(45))
            .Build();

        upgradeEngine.PerformUpgrade();

        testProvider.Connection.CommandsIssued.Count.ShouldNotBe(0);
        testProvider.Connection.CommandsIssued.Last().CommandTimeout.ShouldBe(45);
    }

    [Fact]
    public void WithExecutionTimeout_Should_Not_Set_CommandTimeout_Property_For_Null()
    {
        var testProvider = new TestProvider();
        var upgradeEngine = testProvider.Builder
            .WithScript("testscript", "test")
            .WithExecutionTimeout(null)
            .Build();

        upgradeEngine.PerformUpgrade();

        testProvider.Connection.CommandsIssued.Count.ShouldNotBe(0);
        testProvider.Connection.CommandsIssued.Last().CommandTimeout.ShouldBe(0);
    }

    [Fact]
    public void WithExecutionTimeout_Should_Not_Allow_Negative_Timeout_Values()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            var testProvider = new TestProvider();
            var upgradeEngine = testProvider.Builder
                .WithScript("testscript", "test")
                .WithExecutionTimeout(TimeSpan.FromSeconds(-5))
                .Build();
        });
    }
}
