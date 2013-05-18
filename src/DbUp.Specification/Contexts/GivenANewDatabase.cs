using System;
using DbUp.Engine;
using System.Collections.Generic;
using DbUp.Engine.Transactions;
using NSubstitute;

namespace DbUp.Specification.Contexts
{
	public class GivenANewDatabase : EmptyDatabase
	{
        public override void BeforeEach()
        {
            base.BeforeEach();

            AllScripts = new List<SqlScript>
                             {
                                 new SqlScript("0001.sql", ""),
                                 new SqlScript("0004.sql", ""),
                                 new SqlScript("0002.sql", "")
                             };

            ScriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(AllScripts);
            VersionTracker.GetExecutedScripts().Returns(new string[] {});
        }
	}
}

