using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;
using DbUp.Engine.Transactions;
using DbUp.Helpers;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// A standard implementation of the IScriptExecutor interface that executes against a SQL Server 
    /// database.
    /// </summary>
    public sealed class SqlScriptExecutor : IScriptExecutor
    {
        private readonly Func<IConnectionManager> connectionManagerFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly Func<bool> variablesEnabled;
        private readonly SqlStatementsContainer statementContainer;

        /// <summary>
        /// SQLCommand Timeout in seconds. If not set, the default SQLCommand timeout is not changed.
        /// </summary>
        public int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="SqlScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="statementContainer">Query provider.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        public SqlScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, Func<SqlStatementsContainer> statementContainer, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
            this.connectionManagerFactory = connectionManagerFactory;
            this.statementContainer = statementContainer();
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
        /// Verifies the existence of targeted schema. If schema is not verified, will check for the existence of the dbo schema.
        /// </summary>
        public void VerifySchema()
        {
            if (string.IsNullOrEmpty(statementContainer.Scheme)) return;

            connectionManagerFactory().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                var sqlRunner = new AdHocSqlRunner(dbCommandFactory, statementContainer.Scheme, () => true);

                sqlRunner.ExecuteNonQuery(string.Format(statementContainer.CreateSchemeIfNotExists(), statementContainer.Scheme));
            });
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
            if (statementContainer.Scheme != null && !variables.ContainsKey("schema"))
                variables.Add("schema", SqlObjectParser.QuoteSqlObjectName(statementContainer.Scheme));

            log().WriteInformation("Executing SQL Server script '{0}'", script.Name);

            var contents = script.Contents;
            if (string.IsNullOrEmpty(statementContainer.Scheme))
                contents = new StripSchemaPreprocessor().Process(contents);
            if (variablesEnabled())
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);
            contents = (scriptPreprocessors ?? new IScriptPreprocessor[0])
                .Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));

            var connectionManager = connectionManagerFactory();
            var scriptStatements = connectionManager.SplitScriptIntoCommands(contents);
            var index = -1;
            string executingStatement = String.Empty;
            try
            {
                connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    foreach (var statement in scriptStatements)
                    {
                        index++;
                        executingStatement = statement;
                        using (var command = dbCommandFactory())
                        {
                            command.CommandText = statement;
                            if (ExecutionTimeoutSeconds != null)
                                command.CommandTimeout = ExecutionTimeoutSeconds.Value;
                            if (connectionManager.IsScriptOutputLogged)
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    Log(reader);
                                }
                            }
                            else
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                });
            }
            catch (SqlException sqlException)
            {
                log().WriteError("SQL exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0};    Block line: {1};    Procedure: {2};{5}SQL Exception Number: {3};    Message: {4}{5}", index, sqlException.LineNumber, sqlException.Procedure, sqlException.Number, sqlException.Message, Environment.NewLine);
                log().WriteInformation(executingStatement + Environment.NewLine);
                throw;
            }
            catch (DbException sqlException)
            {
                log().WriteError("DB exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, sqlException.ErrorCode, sqlException.Message);
                log().WriteError(sqlException.ToString());
                log().WriteInformation(executingStatement + Environment.NewLine);
                throw;
            }
            catch (Exception ex)
            {
                log().WriteError("Exception has occured in script: '{0}'", script.Name);
                log().WriteError(ex.ToString());
                log().WriteInformation(executingStatement + Environment.NewLine);
                throw;
            }
        }

        private void Log(IDataReader reader)
        {
            do
            {
                var names = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    names.Add(reader.GetName(i));
                }

                if (names.Count == 0)
                    return;

                var lines = new List<List<string>>();
                while (reader.Read())
                {
                    var line = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        value = value == DBNull.Value ? null : value.ToString();
                        line.Add((string) value);
                    }
                    lines.Add(line);
                }

                string format = "";
                int totalLength = 0;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    int maxLength = lines.Max(l => (l[i] ?? "").Length) + 2;
                    format += " {" + i + ", " + maxLength + "} |";
                    totalLength += (maxLength + 3);
                }
                format = "|" + format;
                totalLength += 1;

                log().WriteInformation(new string('-', totalLength));
                log().WriteInformation(format, names.ToArray());
                log().WriteInformation(new string('-', totalLength));
                foreach (var line in lines)
                {
                    log().WriteInformation(format, line.ToArray());
                }
                log().WriteInformation(new string('-', totalLength));
                log().WriteInformation("\r\n");
            } while (reader.NextResult());
        }
    }
}
