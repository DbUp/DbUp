using System;
using System.Collections.Generic;
using DbUp.ScriptProviders;
using NSubstitute;
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
		public const string ConnectionString = "sqlite:memory:";
		
		public GivenAnOutOfDateDatabase ()
		{
			ScriptProvider = Substitute.For<IScriptProvider> ();
			VersionTracker = Substitute.For<IJournal> ();
			ScriptExecutor = Substitute.For<IScriptExecutor> ();
			Log = Substitute.For<ILog> ();
			
			ScriptProvider.GetScripts().Returns(new List<SqlScript>() {
				new SqlScript("0001.southwind.sql", "--LITTLE BOBBY DROP TABLES WAS HERE."),
				new SqlScript("0002.southwind.sql", "CREATE TABLE USERS --AGAIN")
			});
			
			VersionTracker.GetExecutedScripts(ConnectionString, Log).Returns(new [] {"0001.southwind.sql"});
			
			DbUpgrader = new DatabaseUpgrader(ConnectionString, ScriptProvider, VersionTracker, ScriptExecutor);
		}
	}
}

