#region Using(s)

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Preprocessors;
using DbUp.Helpers;

#endregion

namespace DbUp.Support.SqlServer
{
    /// <summary>
    ///     A standard implementation of the IScriptExecutor interface that executes against a SQL Server
    ///     database.
    /// </summary>
    public sealed class SqlScriptExecutor : ISqlScriptExecutor
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="SqlScriptExecutor" /> class.
        /// </summary>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        public SqlScriptExecutor(string schema)
        {
            Schema = schema;
            ScriptPreprocessors = new List<ISqlScriptPreprocessor>();
        }

        /// <summary>
        ///     Script Preprocessors in addition to variable substitution
        /// </summary>
        public IList<ISqlScriptPreprocessor> ScriptPreprocessors { get; set; }

  
        /// <summary>
        ///     SQLCommand Timeout in seconds. If not set, the default SQLCommand timeout is not changed.
        /// </summary>
        public int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        ///     Database Schema, should be null if database does not support schemas
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Verifies the existence of targeted schema. If schema is not verified, will check for the existence of the dbo schema.
        /// </summary>
        public void VerifySchema(UpgradeConfiguration upgradeConfiguration)
        {
            if (string.IsNullOrEmpty(Schema)) return;

            var sqlRunner = new AdHocSqlRunner(upgradeConfiguration.ConnectionFactory, Schema, () => true);

            sqlRunner.ExecuteNonQuery(string.Format(
                @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA {0}')", Schema));
        }

        private static IEnumerable<string> SplitByGoStatements(string script)
        {
            string[] scriptStatements =
                Regex.Split(script, "^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                     .Select(x => x.Trim())
                     .Where(x => x.Length > 0)
                     .ToArray();

            return scriptStatements;
        }

        /// <summary>
        ///     Executes the specified script against a database at a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="variables">Variables to replace in the script</param>
        public void Execute(SqlScript script, UpgradeConfiguration upgradeConfiguration)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            if (upgradeConfiguration.Variables != null)
                variables = new Dictionary<string, string>(upgradeConfiguration.Variables);

            if (Schema != null && !variables.ContainsKey("schema"))
                variables.Add("schema", Schema);


            string contents = script.Contents;

            if (string.IsNullOrEmpty(Schema))
                contents = new StripSchemaPreprocessor().Process(contents);

            if (upgradeConfiguration.VariablesEnabled)
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);

            contents = (ScriptPreprocessors ?? new ISqlScriptPreprocessor[0])
                .Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));

            IEnumerable<string> scriptStatements = SplitByGoStatements(contents);
            int index = -1;
            try
            {
                using (IDbConnection connection = upgradeConfiguration.ConnectionFactory())
                {
                    connection.Open();

                    foreach (string statement in scriptStatements)
                    {
                        index++;
                        IDbCommand command = connection.CreateCommand();
                        command.CommandText = statement;
                        if (ExecutionTimeoutSeconds != null)
                            command.CommandTimeout = ExecutionTimeoutSeconds.Value;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                upgradeConfiguration.Log.WriteInformation("SQL exception has occured in script: '{0}'", script.Name);
                upgradeConfiguration.Log.WriteError("Script block number: {0}; Block line {1}; Message: {2}", index, sqlException.LineNumber, sqlException.Procedure, sqlException.Number, sqlException.Message);
                upgradeConfiguration.Log.WriteError(sqlException.ToString());
                throw;
            }
            catch (DbException sqlException)
            {
                upgradeConfiguration.Log.WriteInformation("DB exception has occured in script: '{0}'", script.Name);
                upgradeConfiguration.Log.WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, sqlException.ErrorCode, sqlException.Message);
                upgradeConfiguration.Log.WriteError(sqlException.ToString());
                throw;
            }
            catch (Exception ex)
            {
                upgradeConfiguration.Log.WriteInformation("Exception has occured in script: '{0}'", script.Name);
                upgradeConfiguration.Log.WriteError(ex.ToString());
                throw;
            }
        }
    }

    public sealed class SqlScriptRunner
    {
    }
}