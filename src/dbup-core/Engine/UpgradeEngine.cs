using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine.Transactions;

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
            return configuration.ConnectionManager.TryConnect(configuration.Log, out errorMessage);
        }

        /// <summary>
        /// Performs the database upgrade.
        /// </summary>
        public DatabaseUpgradeResult PerformUpgrade()
        {
            var executed = new List<SqlScript>();
            configuration.Log.WriteInformation("Beginning database upgrade");



            string executedScriptName = null;
            try
            {
                List<SqlScript> scriptsToExecute = GetScriptsToExecuteInsideOperation();

                if (scriptsToExecute.Count == 0)
                {
                    configuration.Log.WriteInformation("No new scripts need to be executed - completing.");
                    return new DatabaseUpgradeResult(executed, true, null);
                }

                configuration.ScriptExecutor.VerifySchema();


                foreach (IGrouping<string, SqlScript> sqlScripts in scriptsToExecute.GroupBy(x => x.SqlScriptOptions.Group))
                {
                    TransactionMode transactionMode = sqlScripts.First().SqlScriptOptions.TransactionMode;
                    Func<SqlScript, int> sort = sqlScripts.First().SqlScriptOptions.Sort;

                    var sorted = sort == null ? sqlScripts.ToList() : sqlScripts.OrderBy(sort).ToList();

                    configuration.Log.WriteInformation("");
                    configuration.Log.WriteInformation($"Begin executing script group [{sqlScripts.Key}]");
                    if (sqlScripts.Any(x => x.SqlScriptOptions.TransactionMode != transactionMode))
                    {
                        throw new Exception($"Not all scripts in group [{sqlScripts.Key}] share the same TransactionMode");
                    }
                    using (configuration.ConnectionManager.OperationStarting(configuration.Log, executed, transactionMode))
                    {

                        foreach (SqlScript script in sorted)
                        {
                            executedScriptName = script.Name;

                            configuration.ScriptExecutor.Execute(script, transactionMode, configuration.Variables, configuration.DeploymentId);

                            executed.Add(script);
                        }
                    }

                    
                    configuration.Log.WriteInformation($"End executing script group [{sqlScripts.Key}]");
                    configuration.Log.WriteInformation("");
                }

                configuration.Log.WriteInformation("Upgrade successful");
                return new DatabaseUpgradeResult(executed, true, null);

            }
            catch (Exception ex)
            {
                ex.Data.Add("Error occurred in script: ", executedScriptName);
                configuration.Log.WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
                return new DatabaseUpgradeResult(executed, false, ex);
            }
        }

        /// <summary>
        /// Returns a list of scripts that will be executed when the upgrade is performed
        /// </summary>
        /// <returns>The scripts to be executed</returns>
        public List<SqlScript> GetScriptsToExecute()
        {
            using (configuration.ConnectionManager.OperationStarting(configuration.Log, new List<SqlScript>(), TransactionMode.SingleTransaction))
            {
                return GetScriptsToExecuteInsideOperation();
            }
        }

        private List<SqlScript> GetScriptsToExecuteInsideOperation()
        {
            var allScripts = configuration.ScriptProviders
                .SelectMany(scriptProvider => scriptProvider.GetScripts(configuration.ConnectionManager));

            var executedScriptNames = new HashSet<ExecutedSqlScript>(configuration.Journal.GetExecutedScripts());

            var sorted = allScripts.OrderBy(s => s.SqlScriptOptions.RunGroupOrder)
                .ThenBy(s => s.Name, configuration.ScriptNameComparer);

                
            var filtered = configuration.ScriptFilter.Filter(sorted, executedScriptNames, configuration.ScriptNameComparer);
            return filtered.ToList();
        }

        private TKey SortFunc<TKey>(SqlScript arg)
        {
            throw new NotImplementedException();
        }

        public List<ExecutedSqlScript> GetExecutedScripts()
        {
            using (configuration.ConnectionManager.OperationStarting(configuration.Log, new List<SqlScript>(), TransactionMode.SingleTransaction))
            {
                return configuration.Journal.GetExecutedScripts()
                    .ToList();
            }
        }

        ///<summary>
        /// Creates version record for any new migration scripts without executing them.
        /// Useful for bringing development environments into sync with automated environments
        ///</summary>
        ///<returns></returns>
        public DatabaseUpgradeResult MarkAsExecuted()
        {
            var marked = new List<SqlScript>();
            using (configuration.ConnectionManager.OperationStarting(configuration.Log, marked, TransactionMode.SingleTransaction))
            {
                try
                {
                    var scriptsToExecute = GetScriptsToExecuteInsideOperation();

                    foreach (var script in scriptsToExecute)
                    {
                        configuration.ConnectionManager.ExecuteCommandsWithManagedConnection(
                            connectionFactory => configuration.Journal.StoreExecutedScript(script, null, connectionFactory));
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

        public DatabaseUpgradeResult MarkAsExecuted(string latestScript)
        {
            var marked = new List<SqlScript>();
            using (configuration.ConnectionManager.OperationStarting(configuration.Log, marked, TransactionMode.SingleTransaction))
            {
                try
                {
                    var scriptsToExecute = GetScriptsToExecuteInsideOperation();

                    foreach (var script in scriptsToExecute)
                    {
                        configuration.ConnectionManager.ExecuteCommandsWithManagedConnection(
                            commandFactory => configuration.Journal.StoreExecutedScript(script,null, commandFactory));
                        configuration.Log.WriteInformation("Marking script {0} as executed", script.Name);
                        marked.Add(script);
                        if (script.Name.Equals(latestScript))
                        {
                            break;
                        }
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
}