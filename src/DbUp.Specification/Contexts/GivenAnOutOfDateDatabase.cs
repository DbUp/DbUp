using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification.Contexts
{
	public class GivenAnOutOfDateDatabase : EmptyDatabase
	{
        [SetUp]
        public override void BeforeEach()
        {
            base.BeforeEach();
            AllScripts = new List<SqlScript>
                             {
                                 new SqlScript("0001.southwind.sql", "--LITTLE BOBBY DROP TABLES WAS HERE."),
                                 new SqlScript("0002.southwind.sql", "CREATE TABLE USERS --AGAIN")
                             };

            ScriptProvider.GetScripts(Arg.Any<IConnectionManager>()).Returns(AllScripts);
            VersionTracker.GetExecutedScripts().Returns(new[] {"0001.southwind.sql"});
        }
	}
}