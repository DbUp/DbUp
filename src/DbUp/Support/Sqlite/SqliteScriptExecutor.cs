using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;

namespace DbUp.Support.SQLite
{
    /// <summary>
    /// A standard implementation of the IScriptExecutor interface that executes against a SQLite database
    /// database.
    /// </summary>
    public sealed class SQLiteScriptExecutor : IScriptExecutor
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly Func<bool> variablesEnabled;

        /// <summary>
        /// SQLCommand Timeout in seconds. If not set, the default SQLCommand timeout is not changed.
        /// </summary>
        public int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="SQLiteScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="log"></param>
        /// <param name="variablesEnabled"></param>
        /// <param name="scriptPreprocessors"></param>
        public SQLiteScriptExecutor(Func<IDbConnection> connectionFactory, Func<IUpgradeLog> log, Func<bool> variablesEnabled, IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            this.connectionFactory = connectionFactory;
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
        }

        /// <summary>
        /// Sqlite does support schema and always verified.
        /// </summary>
        public void VerifySchema()
        {   
        }

        /// <summary>
        /// Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void Execute(SqlScript script)
        {
            Execute(script, null);
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

            log().WriteInformation("Executing SQLite script '{0}'", script.Name);

            var contents = script.Contents;
            if (variablesEnabled())
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);
            contents = (scriptPreprocessors ?? new IScriptPreprocessor[0])
                .Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));
            var scriptStatements = SplitBySemiColonStatements(contents);
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
                        if (ExecutionTimeoutSeconds != null)
                            command.CommandTimeout = ExecutionTimeoutSeconds.Value;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (DbException sqlException)
            {
                log().WriteInformation("DB exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, sqlException.ErrorCode, sqlException.Message);
                log().WriteError(sqlException.ToString());
                throw;
            }
            catch (Exception ex)
            {
                log().WriteInformation("Exception has occured in script: '{0}'", script.Name);
                log().WriteError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Sqlite statements seprator is ; (see http://www.sqlite.org/lang.html)
        /// </summary>
        /// <param name="script">script to split</param>
        /// <returns>lis of statements seperated by semi colon</returns>
        private static IEnumerable<string> SplitBySemiColonStatements(string script)
        {
            var scriptStatements =
                Regex.Split(script, "^\\s*;\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            return scriptStatements;
        }
    }
}
