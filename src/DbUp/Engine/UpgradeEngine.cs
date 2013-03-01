using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;

namespace DbUp.Engine
{
    /// <summary>
    /// This class orchestrates the database upgrade process.
    /// </summary>
    public class UpgradeEngine
    {
        private readonly UpgradeConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeEngine"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public UpgradeEngine(UpgradeConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Determines whether the database is out of date and can be upgraded.
        /// </summary>
        public bool IsUpgradeRequired()
        {
            return GetScriptsToExecute().Count() != 0;
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
                using (var connection = configuration.ConnectionFactory())
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
            var executed = new List<IScript>();
            try
            {
                configuration.Log.WriteInformation("Beginning database upgrade");

                var scriptsToExecute = GetScriptsToExecute();

                if (scriptsToExecute.Count == 0)
                {
                    configuration.Log.WriteInformation("No new scripts need to be executed - completing.");
                    return new DatabaseUpgradeResult(executed, true, null);
                }

                configuration.SqlScriptExecutor.VerifySchema(configuration);

                foreach (var script in scriptsToExecute)
                {
                    script.Execute(configuration);

                    configuration.Journal.StoreExecutedScript(script);
                    executed.Add(script);
                }

                configuration.Log.WriteInformation("Upgrade successful");
                return new DatabaseUpgradeResult(executed, true, null);
            }
            catch (Exception ex)
            {
                configuration.Log.WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
                return new DatabaseUpgradeResult(executed, false, ex);
            }
        }

        private List<IScript> GetScriptsToExecute()
        {
            var allScripts = configuration.ScriptProviders.SelectMany(scriptProvider => scriptProvider.GetScripts());
            var executedScripts = configuration.Journal.GetExecutedScripts();

            return allScripts.Where(s => executedScripts.All(y => y != s.Name)).ToList();
        }

        ///<summary>
        /// Creates version record for any new migration scripts without executing them.
        /// Useful for bringing development environments into sync with automated environments
        ///</summary>
        ///<returns></returns>
        public DatabaseUpgradeResult MarkAsExecuted()
        {
            var marked = new List<IScript>();
            try
            {
                var scriptsToExecute = GetScriptsToExecute();

                foreach (var script in scriptsToExecute)
                {
                    configuration.Journal.StoreExecutedScript(script);
                    configuration.Log.WriteInformation("Marking script {0} as executed", script.Name);
                    marked.Add(script);
                }

                configuration.Log.WriteInformation("Script marking successful");
                return new DatabaseUpgradeResult(marked, true, null);
            }
            catch (Exception ex)
            {
                configuration.Log.WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
                return new DatabaseUpgradeResult(marked, false, ex);
            }
        }
    }
}