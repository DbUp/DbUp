using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.ScriptProviders;

namespace DbUp.Execution
{
    /// <summary>
    /// A standard implementation of the IScriptExecutor interface that executes against a SQL Server 
    /// database.
    /// </summary>
    public sealed class SqlScriptExecutor : IScriptExecutor
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly ILog log;

        ///<summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        ///</summary>
        ///<param name="connectionString">The connection string representing the database to act against.</param>
        public SqlScriptExecutor(string connectionString) : this(() => new SqlConnection(connectionString), new ConsoleLog())
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="log">The logging mechanism.</param>
        public SqlScriptExecutor(Func<IDbConnection> connectionFactory, ILog log)
        {
            this.connectionFactory = connectionFactory;
            this.log = log;
        }

        private static IEnumerable<string> SplitByGoStatements(string script)
        {
            var scriptStatements = Regex.Split(script, "^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                                       .Select(x => x.Trim())
                                       .Where(x => x.Length > 0)
                                       .ToArray();
            return scriptStatements;
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void Execute(SqlScript script)
        {
            log.WriteInformation("Executing SQL Server script '{0}'", script.Name);
            
            var scriptStatements = SplitByGoStatements(script.Contents);
            var index = -1;
            try
            {
                using (var connection = connectionFactory())
                {
                    connection.Open();

                    foreach (var statement in scriptStatements)
                    {
                        index++;
                        var command = connection.CreateCommand();
                        command.CommandText = statement;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                log.WriteInformation("SQL exception has occured in script: '{0}'", script.Name);
                log.WriteError("Script block number: {0}; Block line {1}; Message: {2}", index, sqlException.LineNumber, sqlException.Procedure, sqlException.Number, sqlException.Message);
                log.WriteError(sqlException.ToString());
                throw;
            }
            catch (DbException sqlException)
            {
                log.WriteInformation("DB exception has occured in script: '{0}'", script.Name);
                log.WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, sqlException.ErrorCode, sqlException.Message);
                log.WriteError(sqlException.ToString());
                throw;
            }
            catch (Exception ex)
            {
                log.WriteInformation("Exception has occured in script: '{0}'", script.Name);
                log.WriteError(ex.ToString());
                throw;
            }
        }
    }
}