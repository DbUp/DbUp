using System;
using DbUp.Specification.Contexts;
using System.Collections.Generic;
using DbUp.ScriptProviders;
using NSubstitute;
using System.Linq;

namespace DbUp.Specification
{
	public class GivenAnUptoDateDatabase : EmptyDatabase
	{
        public override void BeforeEach()
        {
            base.BeforeEach();
            AllScripts = new List<SqlScript>() {
                new SqlScript("1.sql", ""),
                new SqlScript("2.sql", ""),
                new SqlScript("3.sql", "")
            };

            this.ScriptProvider.GetScripts().Returns(AllScripts);
            this.VersionTracker.GetExecutedScripts(ConnectionString, Log).Returns(AllScripts.Select(s=>s.Name));

        }
	}
}

