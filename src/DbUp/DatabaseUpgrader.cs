using System;
using System.Collections.Generic;
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
        public DatabaseUpgrader(string connectionString, IScriptProvider scriptProvider, IJournal versionTracker, IScriptExecutor scriptExecutor)
        {
            this.connectionString = connectionString;
            this.scriptExecutor = scriptExecutor;
            this.versionTracker = versionTracker;
            this.scriptProvider = scriptProvider;
        }

        /// <summary>
        /// Determines whether the database is out of date and can be upgraded.
        /// </summary>
        /// <param name="log">The log.</param>
        public bool IsUpgradeRequired(ILog log)
        {
            var allScripts = scriptProvider.GetScripts();
            var executedScripts = versionTracker.GetExecutedScripts(connectionString, log);

            var scriptsToExecute = allScripts.Where(x => executedScripts.Any(y => y == x.Name)).ToList();
            return scriptsToExecute.Count != 0;
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
                var executedScripts = versionTracker.GetExecutedScripts(connectionString, log);

                var scriptsToExecute = allScripts.Where(x => !executedScripts.Any(y => y == x.Name)).ToList();
                if (scriptsToExecute.Count == 0)
                {
                    log.WriteInformation("No new scripts need to be executed - completing.");
                    return new DatabaseUpgradeResult(executed, true, null);
                }

                foreach (var script in scriptsToExecute)
                {
                    scriptExecutor.Execute(connectionString, script, log);

                    versionTracker.StoreExecutedScript(connectionString, script, log);

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