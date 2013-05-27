using System;
using System.Collections.Generic;
using System.Data;
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
            var executed = new List<SqlScript>();
            try
            {
                if (configuration.TransactionMode == TransactionMode.SingleTransaction)
                {
                    var originalConnectionFactory = configuration.ConnectionFactory;
                    using (var connection = originalConnectionFactory())
                    {
                        connection.Open();
                        using (var tran = connection.BeginTransaction())
                        using (var ownedConection = new OwnedConnection(connection, tran))
                        {
                            try
                            {
                                configuration.ConnectionFactory = () => ownedConection;
                                return PerformUpgradeInternal(executed);
                            }
                            finally
                            {
                                configuration.ConnectionFactory = originalConnectionFactory;                                
                            }
                        }
                    }
                }

                return PerformUpgradeInternal(executed);
            }
            catch (Exception ex)
            {
                configuration.Log.WriteError("Upgrade failed due to an unexpected exception:\r\n{0}", ex.ToString());
                return new DatabaseUpgradeResult(executed, false, ex);
            }
        }

        private DatabaseUpgradeResult PerformUpgradeInternal(List<SqlScript> executed)
        {
            configuration.Log.WriteInformation("Beginning database upgrade");

            var scriptsToExecute = GetScriptsToExecute();

            if (scriptsToExecute.Count == 0)
            {
                configuration.Log.WriteInformation("No new scripts need to be executed - completing.");
                return new DatabaseUpgradeResult(executed, true, null);
            }

            configuration.ScriptExecutor.VerifySchema();

            foreach (var script in scriptsToExecute)
            {
                configuration.ScriptExecutor.Execute(script, configuration.Variables);

                configuration.Journal.StoreExecutedScript(script);

                executed.Add(script);
            }

            configuration.Log.WriteInformation("Upgrade successful");
            return new DatabaseUpgradeResult(executed, true, null);
        }

        /// <summary>
        /// Returns a list of scripts that will be executed when the upgrade is performed
        /// </summary>
        /// <returns>The scripts to be executed</returns>
        public List<SqlScript> GetScriptsToExecute()
        {
            var allScripts = configuration.ScriptProviders.SelectMany(scriptProvider => scriptProvider.GetScripts(configuration.ConnectionFactory));
            var executedScripts = configuration.Journal.GetExecutedScripts();

            return allScripts.Where(s => !executedScripts.Any(y => y == s.Name)).ToList();
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

    internal class OwnedConnection : IDbConnection
    {
        private readonly IDbConnection connection;
        private readonly IDbTransaction transaction;

        public OwnedConnection(IDbConnection connection, IDbTransaction transaction)
        {
            this.connection = connection;
            this.transaction = transaction;
        }

        public void Dispose()
        {
        }

        public IDbTransaction BeginTransaction()
        {
            return transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return transaction;
        }

        public void Close()
        {
        }

        public void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        public IDbCommand CreateCommand()
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            return cmd;
        }

        public void Open()
        {
        }

        public string ConnectionString { get { return connection.ConnectionString; } set { connection.ConnectionString = value; } }
        public int ConnectionTimeout { get { return connection.ConnectionTimeout; } }
        public string Database { get { return connection.Database; } }
        public ConnectionState State
        {
            get { return connection.State; }
        }
    }
}