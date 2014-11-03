using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;
using DbUp.Engine.Transactions;
using DbUp.Helpers;

namespace DbUp.Engine
{
    /// <summary>
    /// A standard implementation of the IScriptExecutor interface that executes against a SQL
    /// database.
    /// </summary>
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly Func<IConnectionManager> connectionManagerFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly IObjectParser objectParser;
        private readonly ICreateSchema createSchema;
        private readonly Func<bool> variablesEnabled;

        /// <summary>
        /// SQLCommand Timeout in seconds. If not set, the default SQLCommand timeout is not changed.
        /// </summary>
        public int? ExecutionTimeoutSeconds { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="ScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="objectParser">Quotation formatter for sql string</param>
        /// <param name="createSchema">Command to use to create new schema in verifyschema method</param>
        public ScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled, 
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, IObjectParser objectParser, ICreateSchema createSchema)
        {
            Schema = schema;
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
            this.objectParser = objectParser;
            this.createSchema = createSchema;
            this.connectionManagerFactory = connectionManagerFactory;
        }

        /// <summary>
        /// Database Schema, should be null if database does not support schemas
        /// </summary>
        public string Schema { get; set; }

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
            if (string.IsNullOrEmpty(Schema)) return;

            connectionManagerFactory().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                var sqlRunner = new AdHocSqlRunner(dbCommandFactory, Schema, objectParser, () => true);

                sqlRunner.ExecuteNonQuery(createSchema.Command(Schema));
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
            if (Schema != null && !variables.ContainsKey("schema"))
                variables.Add("schema", objectParser.QuoteSqlObjectName(Schema));

            log().WriteInformation("Executing script '{0}'", script.Name);

            var contents = script.Contents;
            if (string.IsNullOrEmpty(Schema))
                contents = new StripSchemaPreprocessor().Process(contents);
            if (variablesEnabled())
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);
            contents = (scriptPreprocessors??new IScriptPreprocessor[0])
                .Aggregate(contents, (current, additionalScriptPreprocessor) => additionalScriptPreprocessor.Process(current));

            var connectionManager = connectionManagerFactory();
            var scriptStatements = connectionManager.SplitScriptIntoCommands(contents);
            var index = -1;
            try
            {
                connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    foreach (var statement in scriptStatements)
                    {
                        index++;
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
