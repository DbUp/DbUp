using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Tests.Contexts;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Concerning
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