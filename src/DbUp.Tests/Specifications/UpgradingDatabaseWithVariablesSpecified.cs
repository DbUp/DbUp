using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Tests.Specifications.Contexts;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Specifications
{
    [TestFixture]
    public class UpgradingDatabaseWithVariablesSpecifiedSubstitutesVariables : GivenScriptsWithVariableSubstitutions
    {
        [Test]
        public void ShouldCallPreprocessor()
        {
            DbUpgrader.PerformUpgrade();

            ScriptExecutor.Received().Execute(Arg.Any<SqlScript>(), Arg.Any<IDictionary<string, string>>());
        }
    }
}