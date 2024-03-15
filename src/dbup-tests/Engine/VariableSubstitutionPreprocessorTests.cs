using System;
using DbUp.Engine;
using DbUp.Tests.TestInfrastructure;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests.Engine;

public class VariableSubstitutionPreprocessorTests
{
    readonly TestProvider testProvider = new();
    DatabaseUpgradeResult result;

    void GivenAScript(string contents)
        => testProvider.Builder.WithScript("testscript", contents);

    void GivenAVariable(string name, string value)
        => testProvider.Builder.WithVariable(name, value);

    void WhenUpgradeIsPerformed()
        => result = testProvider.Builder.Build().PerformUpgrade();

    void ThenTheUpgradeWasSuccessful()
        => result.Successful.ShouldBeTrue();

    void ThenTheUpgradeWasUnsuccessful()
        => result.Successful.ShouldBeFalse();

    void ThenTheCommandWasIssuedWithText(string commandText)
        => testProvider.Log.WriteDbOperations.ShouldContain($"Execute non query command: {commandText}");

    void ThenTheErrorWasAnInvalidOperationException()
        => result.Error.ShouldBeOfType<InvalidOperationException>();

    [Fact]
    public void substitutes_variables_in_body()
        => this.Given(_ => _.GivenAScript("something $somevar$ something"))
            .And(_ => GivenAVariable("somevar", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("something coriander something"))
            .BDDfy();

    [Fact]
    public void substitutes_variables_in_quoted_text()
        => this.Given(_ => _.GivenAScript("'$somevar$'"))
            .And(_ => GivenAVariable("somevar", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("'coriander'"))
            .BDDfy();

    [Fact]
    public void ignores_undefined_variables_in_comments()
        => this.Given(_ => _.GivenAScript("/*$somevar$*/"))
            .And(_ => GivenAVariable("beansprouts", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("/*$somevar$*/"))
            .BDDfy();


    [Fact]
    public void ignores_undefined_variables_in_complex_comments()
        => this.Given(_ => _.GivenAScript("/*/**/$somevar$*/"))
            .And(_ => GivenAVariable("beansprouts", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("/*/**/$somevar$*/"))
            .BDDfy();

    [Fact]
    public void ignores_undefined_variable_in_line_comment()
        => this.Given(_ => _.GivenAScript("--$somevar$"))
            .And(_ => GivenAVariable("beansprouts", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("--$somevar$"))
            .BDDfy();

    [Fact]
    public void throws_for_undefined_variable()
        => this.Given(_ => _.GivenAScript("$somevar$"))
            .And(_ => GivenAVariable("beansprouts", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasUnsuccessful())
            .And(_ => _.ThenTheErrorWasAnInvalidOperationException())
            .BDDfy();

    [Fact]
    public void ignores_if_whitespace_between_dollars()
        => this.Given(_ => _.GivenAScript("$some var$"))
            .And(_ => GivenAVariable("some var", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("$some var$"))
            .BDDfy();


    [Fact]
    public void ignores_if_newline_between_dollars()
        => this.Given(_ => _.GivenAScript("$some\nvar$"))
            .And(_ => GivenAVariable("somevar", "coriander"))
            .When(_ => _.WhenUpgradeIsPerformed())
            .Then(_ => _.ThenTheUpgradeWasSuccessful())
            .And(_ => _.ThenTheCommandWasIssuedWithText("$some\nvar$"))
            .BDDfy();
}
