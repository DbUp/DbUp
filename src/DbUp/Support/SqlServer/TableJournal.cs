using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a SQL Server database
    /// </summary>
    public class TableJournal : IJournal
    {
        /// <summary>
        /// Object for getting sql strings
        /// </summary>
        protected IQueryProvider QueryProvider;
        private readonly Func<IConnectionManager> connectionManager;
        private readonly Func<IUpgradeLog> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True");
        /// </example>
        public TableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, Func<IQueryProvider> queryFunc )
        {
            this.connectionManager = connectionManager;
            log = logger;
            QueryProvider = queryFunc();
        }

        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns>All executed scripts.</returns>
        public string[] GetExecutedScripts()
        {
            log().WriteInformation("Fetching list of already executed scripts.");
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", QueryProvider.TableName));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = QueryProvider.GetVersionTableExecutedScriptsSql();
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            scripts.Add((string)reader[0]);
                    }
                }
            });

            return scripts.ToArray();
        }
        /// <summary>
        /// Not implemented yet!
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public bool ValidateExecutedScript(SqlScript script)
        {
            return true;
        }

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("Creating the {0} table", QueryProvider.TableName));

                connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = QueryProvider.VersionTableCreationString();

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }

                    log().WriteInformation(string.Format("The {0} table has been created", QueryProvider.TableName));
                });
            }

            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = QueryProvider.VersionTableNewEntry();

                    var scriptNameParam = command.CreateParameter();
                    scriptNameParam.ParameterName = "scriptName";
                    scriptNameParam.Value = script.Name;
                    command.Parameters.Add(scriptNameParam);
                    
                    var appliedParam = command.CreateParameter();
                    appliedParam.ParameterName = "applied";
                    appliedParam.Value = String.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.UtcNow);
                    command.Parameters.Add(appliedParam);

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            });
        }

        private bool DoesTableExist()
        {
            return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = QueryProvider.VersionTableDoesTableExist();
                        command.CommandType = CommandType.Text;
                        command.ExecuteScalar();
                        return true;
                    }
                }
                catch (SqlException)
                {
                    return false;
                }
                catch (DbException)
                {
                    return false;
                }
            });
        }
    }
}