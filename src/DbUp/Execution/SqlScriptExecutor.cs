using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Helpers;
using DbUp.Preprocessors;
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
        private readonly string schema;
        private readonly IScriptPreprocessor[] additionalScriptPreprocessors;

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
        ///<param name="schema">The schema that contains the table.</param>
        public SqlScriptExecutor(string connectionString, string schema) : this(connectionString, new ConsoleLog(), schema)
        {
        }

        ///<summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        ///</summary>
        ///<param name="connectionString">The connection string representing the database to act against.</param>
        ///<param name="log">The logging mechanism.</param>
        public SqlScriptExecutor(string connectionString, ILog log) : this(connectionString, log, "dbo")
        {
        }

        ///<summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        ///</summary>
        ///<param name="connectionString">The connection string representing the database to act against.</param>
        ///<param name="log">The logging mechanism.</param>
        ///<param name="schema">The schema that contains the table.</param>
        ///<param name="additionalScriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        public SqlScriptExecutor(string connectionString, ILog log, string schema, params IScriptPreprocessor[] additionalScriptPreprocessors)
        {
            dbConnectionString = connectionString;
            this.log = log;
            this.schema = schema;
            this.additionalScriptPreprocessors = additionalScriptPreprocessors;
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
            Execute(script, null);
        }

        public void VerifySchema()
        {
            var sqlRunner = new AdHocSqlRunner(dbConnectionString, schema);

            sqlRunner.ExecuteNonQuery(string.Format(
                @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA {0}')", schema));
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="variables">Variables to replace in the script</param>
        public void Execute(SqlScript script, IDictionary<string, string> variables)
        {
            if (variables == null)
                variables = new Dictionary<string, string>();
            if (!variables.ContainsKey("schema"))
                variables.Add("schema", schema);

            log.WriteInformation("Executing SQL Server script '{0}'", script.Name);

            var contents = new VariableSubstitutionPreprocessor(variables).Process(script.Contents);
            contents = additionalScriptPreprocessors.Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));

            var scriptStatements = SplitByGoStatements(contents);
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