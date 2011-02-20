using System;
using DbUp.Specification.Contexts;
using System.Collections.Generic;
using DbUp.ScriptProviders;
using NSubstitute;
using System.Linq;
using NUnit.Framework;

namespace DbUp.Specification
{
	public class GivenAnUptoDateDatabase : EmptyDatabase
	{
        public override void BeforeEach()
        {
            base.BeforeEach();
            var executedScripts = new[] { "0001.sql", "0002.sql", "0004.sql" };
            AllScripts = new List<SqlScript>()
            {
                new SqlScript("0001.sql", ""),
                new SqlScript("0004.sql", ""),
                new SqlScript("0002.sql", "")
            };

            ScriptProvider.GetScripts().Returns(AllScripts);
            VersionTracker.GetExecutedScripts(Log).Returns(executedScripts);
        }
	}
}

