﻿using System;
﻿using System.Collections.Generic;
﻿using DbUp.Engine;
﻿using DbUp.Specification.Contexts;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification.Concerning
{
    [TestFixture]
    public class UpgradingDatabaseWithVariablesSpecified : GivenScriptsWithVariableSubstitutions
    {
        [Test]
        public void ShouldCallPreprocessor()
        {
            DbUpgrader.PerformUpgrade();

            ScriptExecutor.Received().Execute(Arg.Any<SqlScript>(), Arg.Any<IDictionary<string, string>>());
        }
    }
}