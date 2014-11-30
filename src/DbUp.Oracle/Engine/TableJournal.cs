using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Oracle.Engine
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a Oracle database
    /// </summary>
    public class TableJournal : IJournal
    {
        private readonly SqlStatementsContainer statementsProvider;
        private readonly Func<IConnectionManager> connectionManager;
        private readonly Func<IUpgradeLog> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="queryProvider">Container which holds queries and informations about database.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True");
        /// </example>
        public TableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger)
        {
            this.connectionManager = connectionManager;
            log = logger;
            this.statementsProvider = connectionManager().SqlContainer;
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
                log().WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", statementsProvider.TableName));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = statementsProvider.GetVersionTableExecutedScriptsSql();
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
        /// Validate already executed SqlScript with state in database.
        /// </summary>
        /// <param name="script">SqlScript to validate.</param>
        /// <returns>True if SqlScript is valid.</returns>
        public bool ValidateScript(SqlScript script)
        {
            return ValidateExecutedScript(script, null);
        }

        /// <summary>
        /// Get index of failed part in oracle script from journal database table. 
        /// </summary>
        /// <param name="script">Script to get index for failed part.</param>
        /// <returns>Index of failed part</returns>
        public int GetFailedStatementIndex(SqlScript script)
        {
            var exists = DoesTableExist();
            if (exists)
            {
                return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    using (var command = dbCommandFactory())
                    {
                        var oracleSqlStatementsContainer = statementsProvider as OracleStatementsContainer;
                        if (oracleSqlStatementsContainer == null) throw new Exception("No oracle SQL statemenst container setted!");

                        command.CommandText = oracleSqlStatementsContainer.GetFailedScriptIndex();

                        var scriptNameParam = command.CreateParameter();
                        scriptNameParam.ParameterName = "scriptName";
                        scriptNameParam.Value = script.Name;
                        command.Parameters.Add(scriptNameParam);

                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                return Convert.ToInt32(reader[0]);
                        }
                    }
                    return 0;
                });
            }
            return 0;
        }
        /// <summary>
        /// Get hash of sucessfully executed parts of failed oracle script from journal database table. 
        /// This is intendant for validation of already successfully executed parts of Oracle scripts, so developers don't change allready appiled changes.
        /// </summary>
        /// <param name="script">Script to get hash of sucessfully executed parts.</param>
        /// <returns>Index of failed part</returns>
        public int GetFailedStatementHash(SqlScript script)
        {
            var exists = DoesTableExist();
            if (exists)
            {
                return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    using (var command = dbCommandFactory())
                    {
                        var oracleSqlStatementsContainer = statementsProvider as OracleStatementsContainer;
                        if (oracleSqlStatementsContainer == null) throw new Exception("No oracle SQL statemenst container setted!");

                        command.CommandText = oracleSqlStatementsContainer.GetAppliedScriptHash();

                        var scriptNameParam = command.CreateParameter();
                        scriptNameParam.ParameterName = "scriptName";
                        scriptNameParam.Value = script.Name;
                        command.Parameters.Add(scriptNameParam);

                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                return Convert.ToInt32(reader[0]);
                        }
                    }
                    return 0;
                });
            }
            return 0;
        }

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            StoreExecutedScript(script, null, null);
        }

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="failureStatementIndex">Statments that were successfully executed. If null, all statements in script has been successfully executed. </param>
        /// <param name="failureRemark"/>
        public void StoreExecutedScript(SqlScript script, int? failureStatementIndex, string failureRemark)
        {
            var cManagerInstance = connectionManager();
            IEnumerable<string> successfullStatments = cManagerInstance.SplitScriptIntoCommands(script.Contents);

            if (failureStatementIndex != null)
                successfullStatments = successfullStatments.Take(Convert.ToInt32(failureStatementIndex));

            var exists = DoesTableExist();
            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                if (!exists)
                {
                    log().WriteInformation(string.Format("Creating the {0} table", statementsProvider.TableName));

                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = statementsProvider.VersionTableCreationString();

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }

                    log().WriteInformation(string.Format("The {0} table has been created", statementsProvider.TableName));
                }
                else
                {
                    using (var command = dbCommandFactory())
                    {
                        var oracleQueryProvider = statementsProvider as OracleStatementsContainer;
                        if (oracleQueryProvider == null) throw new Exception("No such query provider!");
                        command.CommandText = oracleQueryProvider.DeleteFailedScriptIndex();

                        var scriptNameParam = command.CreateParameter();
                        scriptNameParam.ParameterName = "scriptName";
                        scriptNameParam.Value = script.Name;
                        command.Parameters.Add(scriptNameParam);

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }


                using (var command = dbCommandFactory())
                {
                    command.CommandText = statementsProvider.VersionTableNewEntry();

                    var scriptNameParam = command.CreateParameter();
                    scriptNameParam.ParameterName = "scriptName";
                    scriptNameParam.Value = script.Name;
                    command.Parameters.Add(scriptNameParam);

                    var appliedParam = command.CreateParameter();
                    appliedParam.ParameterName = "applied";
                    appliedParam.Value = String.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.UtcNow);
                    command.Parameters.Add(appliedParam);

                    var successfullStatementIndexParam = command.CreateParameter();
                    successfullStatementIndexParam.ParameterName = "failureStatementIndex";
                    successfullStatementIndexParam.Value = failureStatementIndex;
                    command.Parameters.Add(successfullStatementIndexParam);

                    var failureRemarkParam = command.CreateParameter();
                    failureRemarkParam.ParameterName = "failureRemark";
                    failureRemarkParam.Value = failureRemark;
                    command.Parameters.Add(failureRemarkParam);

                    var hashParam = command.CreateParameter();
                    hashParam.ParameterName = "hash";
                    hashParam.Value = CalculateHash(successfullStatments);
                    command.Parameters.Add(hashParam);

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            });
        }
        /// <summary>
        /// Check if already executed part of script has changed.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="successfullyExecutedStatements">Collection of already successfull executed statements of scipt</param>
        /// <returns>Return true if already executed statements of scripts have not changed.</returns>
        internal bool ValidateExecutedScript(SqlScript script, IEnumerable<string> successfullyExecutedStatements)
        {
            if (successfullyExecutedStatements == null)
            {
                var cManagerInstance = connectionManager();
                successfullyExecutedStatements = cManagerInstance.SplitScriptIntoCommands(script.Contents);
            }

            int successfullHash = GetFailedStatementHash(script);
            int scriptsHash = CalculateHash(successfullyExecutedStatements);
            return successfullHash == scriptsHash;
        }

        private bool DoesTableExist()
        {
            return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = statementsProvider.VersionTableDoesTableExist();
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

        private static int CalculateHash(IEnumerable<string> collection)
        {
            if (collection == null || !collection.Any()) return 0;
            return collection.Aggregate(0, (current, entry) => current ^ entry.GetHashCode());
        }
    }
}
