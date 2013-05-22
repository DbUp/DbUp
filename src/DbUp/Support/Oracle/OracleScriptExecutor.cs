using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;

namespace DbUp.Support.Oracle
{
    /// <summary>
    /// An implementation of the IScriptExecutor interface that executes against an Oracle database.
    /// </summary>
    public sealed class OracleScriptExecutor : IScriptExecutor
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly Func<bool> variablesEnabled;

        /// <summary>
        /// DbCommand Timeout in seconds. If not set, the default DbCommand timeout is not changed.
        /// </summary>
        public int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="OracleScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        public OracleScriptExecutor(Func<IDbConnection> connectionFactory, Func<IUpgradeLog> log, Func<bool> variablesEnabled, IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            this.connectionFactory = connectionFactory;
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
        }

        /// <summary>
        /// Verifies the existence of targeted schema. If schema is not verified, will check for the existence of the dbo schema.
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

            log().WriteInformation("Executing Oracle script '{0}'", script.Name);

            var contents = script.Contents;

            if (variablesEnabled())
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);

            contents = (scriptPreprocessors ?? new IScriptPreprocessor[0])
                .Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));

            var scriptStatements = SplitBatches(contents);
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
            catch (DbException dbException)
            {
                log().WriteInformation("DB exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, dbException.ErrorCode, dbException.Message);
                log().WriteError(dbException.ToString());
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
        /// Split batches, identified by a forward-slash on a separate line.
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        private static IEnumerable<string> SplitBatches(string script)
        {
            var scriptStatements =
                Regex.Split(script, "^/\r*$", RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            return scriptStatements;
        }
    }
}