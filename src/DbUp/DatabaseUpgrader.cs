using System;
using System.Collections.Generic;
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
        private readonly string connectionString;
        private readonly IScriptProvider scriptProvider;
        private readonly IJournal versionTracker;
        private readonly IScriptExecutor scriptExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUpgrader"/> class.
        /// </summary>
        public DatabaseUpgrader(string connectionString, IScriptProvider scriptProvider) 
            : this(connectionString, scriptProvider, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUpgrader"/> class.
        /// </summary>
        public DatabaseUpgrader(string connectionString, IScriptProvider scriptProvider, IJournal versionTracker, IScriptExecutor scriptExecutor)
        {
            this.connectionString = connectionString;
            this.scriptExecutor = scriptExecutor ?? new SqlScriptExecutor(connectionString);
            this.versionTracker = versionTracker ?? new TableJournal(connectionString);
            this.scriptProvider = scriptProvider;
        }

        /// <summary>
        /// Determines whether the database is out of date and can be upgraded.
        /// </summary>
        /// <param name="log">The log.</param>
        public bool IsUpgradeRequired(ILog log)
        {
            var allScripts = scriptProvider.GetScripts();
            var executedScripts = versionTracker.GetExecutedScripts(log);

            var scriptsToExecute = allScripts.Where(x => executedScripts.Any(y => y == x.Name)).ToList();
            return scriptsToExecute.Count != 0;
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
                var csb = new SqlConnectionStringBuilder(connectionString);
                csb.Pooling = false;
                csb.ConnectTimeout = 5;

                errorMessage = "";
                using (var connection = new SqlConnection(csb.ConnectionString))
                {
                    connection.Open();

                    new SqlCommand("select 1", connection).ExecuteScalar();
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
        public DatabaseUpgradeResult PerformUpgrade(ILog log)
        {
            var executed = new List<SqlScript>();
            try
            {
                log.WriteInformation("Beginning database upgrade. Connection string is: '{0}'", connectionString);

                var allScripts = scriptProvider.GetScripts();
                var executedScripts = versionTracker.GetExecutedScripts(log);

                var scriptsToExecute = allScripts.Where(x => !executedScripts.Any(y => y == x.Name)).ToList();
                if (scriptsToExecute.Count == 0)
                {
                    log.WriteInformation("No new scripts need to be executed - completing.");
                    return new DatabaseUpgradeResult(executed, true, null);
                }

                foreach (var script in scriptsToExecute)
                {
                    scriptExecutor.Execute(script, log);

                    versionTracker.StoreExecutedScript(script, log);

                    executed.Add(script);
                }

                log.WriteInformation("Upgrade successful");
                return new DatabaseUpgradeResult(executed, true, null);
            }
            catch (Exception ex)
            {
                log.WriteError("Upgrade failed", ex);
                return new DatabaseUpgradeResult(executed, false, ex);
            }
        }
    }
}