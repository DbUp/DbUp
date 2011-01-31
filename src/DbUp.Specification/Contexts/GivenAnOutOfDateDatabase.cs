using System;
using System.Collections.Generic;
using DbUp.ScriptProviders;
using NSubstitute;
using NUnit.Framework;
using DbUp.Journal;
using DbUp.Execution;

namespace DbUp.Specification.Contexts
{
	public class GivenAnOutOfDateDatabase
	{
		public DatabaseUpgrader DbUpgrader;
		public IScriptExecutor ScriptExecutor;
		public IJournal VersionTracker;
		public IScriptProvider ScriptProvider;
		public ILog Log;
		public IEnumerable<SqlScript> AllScripts;
		public const string ConnectionString = "sqlite:memory:";
		
		[SetUp]
		public void SetUp ()
		{
			ScriptProvider = Substitute.For<IScriptProvider> ();
			VersionTracker = Substitute.For<IJournal> ();
			ScriptExecutor = Substitute.For<IScriptExecutor> ();
			Log = Substitute.For<ILog> ();
			AllScripts = new List<SqlScript>() {
				new SqlScript("0001.southwind.sql", "--LITTLE BOBBY DROP TABLES WAS HERE."),
				new SqlScript("0002.southwind.sql", "CREATE TABLE USERS --AGAIN")
			};
			
			ScriptProvider.GetScripts().Returns(AllScripts);
			
			VersionTracker.GetExecutedScripts(ConnectionString, Log).Returns(new [] {"0001.southwind.sql"});
			
			DbUpgrader = new DatabaseUpgrader(ConnectionString, ScriptProvider, VersionTracker, ScriptExecutor);
		}
		
		[TearDown]
		public void TearDown ()
		{
			ScriptExecutor = null;
			ScriptProvider = null;
			VersionTracker = null;
			Log = null;
			DbUpgrader = null;
		}
	}
}

