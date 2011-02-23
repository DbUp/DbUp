using System.Collections.Generic;
using NUnit.Framework;
using DbUp.ScriptProviders;
using DbUp.Journal;
using DbUp.Execution;
using NSubstitute;

namespace DbUp.Specification.Contexts
{
    public abstract class EmptyDatabase
    {
        public DatabaseUpgrader DbUpgrader;
        public IScriptExecutor ScriptExecutor;
        public IJournal VersionTracker;
        public IScriptProvider ScriptProvider;
        public ILog Log;
        public IEnumerable<SqlScript> AllScripts;
        public const string ConnectionString = "sqlite:memory:";

        [SetUp]
        public virtual void BeforeEach()
        {
            ScriptProvider = Substitute.For<IScriptProvider> ();
			VersionTracker = Substitute.For<IJournal> ();
			ScriptExecutor = Substitute.For<IScriptExecutor> ();
			Log = Substitute.For<ILog> ();
            DbUpgrader = new DatabaseUpgrader(ConnectionString, ScriptProvider, VersionTracker, ScriptExecutor, Log);
        }

        [TearDown]
        public virtual void AfterEach()
        {
            ScriptProvider = null;
            VersionTracker = null;
            ScriptExecutor = null;
            Log = null;
            DbUpgrader = null;
        }
    }
}
