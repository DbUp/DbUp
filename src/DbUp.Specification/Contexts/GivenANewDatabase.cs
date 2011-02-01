using System;
using DbUp.Specification.Contexts;
using System.Collections.Generic;
using DbUp.ScriptProviders;
using NSubstitute;
namespace DbUp.Specification
{
	public class GivenANewDatabase : EmptyDatabase
	{
        public override void BeforeEach()
        {
            base.BeforeEach();
            AllScripts = new List<SqlScript>()
            {
                new SqlScript("0001.sql", ""),
                new SqlScript("0004.sql", ""),
                new SqlScript("0002.sql", "")
            };

            ScriptProvider.GetScripts().Returns(AllScripts);
            VersionTracker.GetExecutedScripts(ConnectionString, Log).Returns(new string[] { });
        }
	}
}

