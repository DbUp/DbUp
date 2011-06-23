using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DbUp.Execution;
using DbUp.Journal;
using DbUp.ScriptProviders;

namespace DbUp
{
    /// <summary>
    /// This class orchestrates the database upgrade process.
    /// </summary>
    public class DatabaseUpgrader
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly IScriptProvider scriptProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUpgrader"/> class.
        /// </summary>
        /// <param name="connectionString">The target database connection string.</param>
        /// <param name="scriptProvider">The script provider instance.</param>
        public DatabaseUpgrader(string connectionString, IScriptProvider scriptProvider)
            : this(() => new SqlConnection(connectionString), scriptProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUpgrader"/> class.
        /// </summary>
        /// <param name="connectionFactory">A delegate that creates new connections for the data provider you are using.</param>
        /// <param name="scriptProvider">The script provider instance.</param>
        public DatabaseUpgrader(Func<IDbConnection> connectionFactory, IScriptProvider scriptProvider)
        /// <param name="versionTracker">The version tracking instance.</param>
        /// <param name="scriptExecutor">The script executor instance.</param>
        public DatabaseUpgrader(string connectionString, IScriptProvider scriptProvider, IJournal versionTracker, IScriptExecutor scriptExecutor) 
        public DatabaseUpgrader(string connectionString, IScriptProvider scriptProvider, IJournal versionTracker, IScriptExecutor scriptExecutor) : this(connectionString, scriptProvider, versionTracker, scriptExecutor, null)
            : this(connectionString, scriptProvider, versionTracker, scriptExecutor, null)
        {
            this.connectionFactory = connectionFactory;
            this.scriptProvider = scriptProvider;
            Log = new ConsoleLog();
            ScriptExecutor = new SqlScriptExecutor(connectionFactory, this.Log);
            Journal = new TableJournal(connectionFactory, "dbo", "SchemaVersions", this.Log);
        }

        /// <summary>
        /// Gets or sets the version tracker.
        /// </summary>
        public IJournal Journal { get; set; }

        /// <summary>
        /// Gets or sets an object that will execute scripts against the database.
        /// </summary>
        /// <value>
        /// The script executor.
        /// </value>
        public IScriptExecutor ScriptExecutor { get; set; }

        /// <summary>
        /// Gets or sets an object that will receive log messages.
        /// </summary>
        public ILog Log { get; set; }

        /// <summary>
        /// Determines whether the database is out of date and can be upgraded.
        /// </summary>
        public bool IsUpgradeRequired()
        {
            var allScripts = scriptProvider.GetScripts();
            var executedScripts = Journal.GetExecutedScripts();

            var scriptsToExecute = allScripts.Where(s => ! executedScripts.Contains(s.Name));
            return scriptsToExecute.Count() != 0;
        }

        /// <summary>
        /// Tries to connect to the database.
        /// </summary>
        /// <param name="errorMessage">Any error message encountered.</param>
        /// <returns></returns>
        public bool TryConnect(out string errorMessage)
        {
            try
            {
                errorMessage = "";
                using (var connection = connectionFactory())
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "select 1";
                    command.ExecuteScalar();
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Performs the database upgrade.
        /// </summary>
        public DatabaseUpgradeResult PerformUpgrade()
        {
            var executed = new List<SqlScript>();
            try
            {
                Log.WriteInformation("Beginning database upgrade");

                var scriptsToExecute = GetScriptsToExecute();

                if (scriptsToExecute.Count == 0)
                {
                    Log.WriteInformation("No new scripts need to be executed - completing.");
                    return new DatabaseUpgradeResult(executed, true, null);
                }

                foreach (var script in scriptsToExecute)
                {
                    ScriptExecutor.Execute(script);

                    Journal.StoreExecutedScript(script);

                    executed.Add(script);
                }

                Log.WriteInformation("Upgrade successful");
                return new DatabaseUpgradeResult(executed, true, null);
            }
            catch (Exception ex)
            {
                Log.WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
                return new DatabaseUpgradeResult(executed, false, ex);
            }
        }

        private List<SqlScript> GetScriptsToExecute()
        {
            var allScripts = scriptProvider.GetScripts();
            var executedScripts = Journal.GetExecutedScripts();

            return allScripts.Where(x => !executedScripts.Any(y => y == x.Name)).ToList();
        }

        ///<summary>
        /// Creates version record for any new migration scripts without executing them.
        /// Useful for bringing development environments into sync with automated environments
        ///</summary>
        ///<returns></returns>
        public DatabaseUpgradeResult MarkAsExecuted()
        {
            var marked = new List<SqlScript>();
            try
            {
                var scriptsToExecute = GetScriptsToExecute();

                foreach (var script in scriptsToExecute)
                {
                    Journal.StoreExecutedScript(script);
                    Log.WriteInformation("Marking script {0} as executed", script.Name);
                    marked.Add(script);
                }

                Log.WriteInformation("Script marking successful");
                return new DatabaseUpgradeResult(marked, true, null);
            }
            catch (Exception ex)
            {
                Log.WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
                return new DatabaseUpgradeResult(marked, false, ex);
            }
        }
    }
}