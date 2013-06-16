using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using NSubstitute;

namespace DbUp.Tests.Contexts
{
	public class GivenAnUptoDateDatabase : EmptyDatabase
	{
        public override void BeforeEach()
        {
            base.BeforeEach();
            var executedScripts = new[] { "0001.sql", "0002.sql", "0004.sql" };
            AllScripts = new List<SqlScript>
            {
                new SqlScript("0001.sql", ""),
                new SqlScript("0004.sql", ""),
                new SqlScript("0002.sql", "")
            };

            ScriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(AllScripts);
            VersionTracker.GetExecutedScripts().Returns(executedScripts);
        }
	}
}