using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.Support.SqlServer;
using MySql.Data.MySqlClient;

namespace DbUp.MySql
{
    class MySqlScriptExecutor : IScriptExecutor
    {
        private readonly Func<IConnectionManager> connectionManagerFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly Func<bool> variablesEnabled;

        public string Schema { get; set; }

        public MySqlScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled, IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            Schema = schema;
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
            this.connectionManagerFactory = connectionManagerFactory;
        }

        public void Execute(SqlScript script)
        {
            Execute(script, null);
        }

        public void Execute(SqlScript script, IDictionary<string, string> variables)
        {
            if(variables == null)
                variables = new Dictionary<string, string>();
            if(Schema != null && !variables.ContainsKey("schema"))
                variables.Add("schema", MySqlObjectParser.QuoteMySqlObjectName(Schema));

            log().WriteInformation("Executing MySql script '{0}'", script.Name);

            var contents = script.Contents;
            if (string.IsNullOrEmpty(Schema))
                contents = new StripSchemaPreprocessor().Process(contents);
            if (variablesEnabled())
                contents = new VariableSubstitutionPreprocessor(variables).Process(contents);
            contents = (scriptPreprocessors ?? new IScriptPreprocessor[0])
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
            catch (MySqlException mySqlException)
            {
                log().WriteInformation("SQL exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0}; Message: {1}", index, mySqlException.Source, mySqlException.Number, mySqlException.Message);
                log().WriteError(mySqlException.ToString());
                throw;
            }
            catch (DbException mySqlException)
            {
                log().WriteInformation("DB exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, mySqlException.ErrorCode, mySqlException.Message);
                log().WriteError(mySqlException.ToString());
                throw;
            }
            catch (Exception ex)
            {
                log().WriteInformation("Exception has occured in script: '{0}'", script.Name);
                log().WriteError(ex.ToString());
                throw;
            }
        }

        public void VerifySchema()
        {
            if (string.IsNullOrEmpty(Schema)) return;

            connectionManagerFactory().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                var sqlRunner = new AdHocSqlRunner(dbCommandFactory, Schema, () => true);

                sqlRunner.ExecuteNonQuery(string.Format(
                    @"CREATE DATABASE IF NOT EXISTS {0};", Schema));
            });
        }

        public int? ExecutionTimeoutSeconds { get; set; }

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
                        line.Add((string)value);
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
