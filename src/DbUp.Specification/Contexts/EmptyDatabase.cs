using System.Collections.Generic;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using NUnit.Framework;
using NSubstitute;

namespace DbUp.Specification.Contexts
{
    public abstract class EmptyDatabase
    {
        public UpgradeEngine DbUpgrader;
        public ISqlScriptExecutor SqlScriptExecutor;
        public IJournal VersionTracker;
        public IScriptProvider ScriptProvider;
        public IUpgradeLog Log;
        public IEnumerable<SqlScript> AllScripts;
        public ISqlScriptPreprocessor SqlScriptPreprocessor;
        public const string ConnectionString = "sqlite:memory:";

        [SetUp]
        public virtual void BeforeEach()
        {
            ScriptProvider = Substitute.For<IScriptProvider> ();
			VersionTracker = Substitute.For<IJournal> ();
			SqlScriptExecutor = Substitute.For<ISqlScriptExecutor> ();
            SqlScriptPreprocessor = Substitute.For<ISqlScriptPreprocessor>();

			Log = Substitute.For<IUpgradeLog> ();

            var config = new UpgradeConfiguration();


            config.ScriptProviders.Add(ScriptProvider);
            config.SqlScriptExecutor = SqlScriptExecutor;
            config.SqlScriptExecutor.ScriptPreprocessors.Add(SqlScriptPreprocessor);
            config.Journal = VersionTracker;
            config.Log = Log;

            DbUpgrader = new UpgradeEngine(config);
        }

        [TearDown]
        public virtual void AfterEach()
        {
            ScriptProvider = null;
            VersionTracker = null;
            SqlScriptExecutor = null;
            Log = null;
            DbUpgrader = null;
        }
    }
}
