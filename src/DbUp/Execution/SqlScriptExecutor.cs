using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.ScriptProviders;

namespace DbUp.Execution
{
    /// <summary>
    /// A standard implementation of the IScriptExecutor interface that executes against a SQL Server 
    /// database using SQL Server SMO.
    /// </summary>
    public sealed class SqlScriptExecutor : IScriptExecutor
    {
        private readonly string dbConnectionString;
        private readonly ILog log;

        ///<summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        ///</summary>
        ///<param name="connectionString">The connection string representing the database to act against.</param>
        public SqlScriptExecutor(string connectionString) : this(connectionString, new ConsoleLog())
        {
        }

        ///<summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        ///</summary>
        ///<param name="connectionString">The connection string representing the database to act against.</param>
        ///<param name="log">The logging mechanism.</param>
        public SqlScriptExecutor(string connectionString, ILog log)
        {
            dbConnectionString = connectionString;
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
                using (var connection = new SqlConnection(dbConnectionString))
                {
                    connection.Open();

                    foreach (var statement in scriptStatements)
                    {
                        index++;
                        var command = new SqlCommand(statement, connection);
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
            catch (Exception ex)
            {
                log.WriteInformation("Exception has occured in script: '{0}'", script.Name);
                log.WriteError(ex.ToString());
                throw;
            }
        }
    }
}