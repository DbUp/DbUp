﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Preprocessors;
using DbUp.Engine.Transactions;

namespace DbUp.Oracle
{
    public class OracleScriptExecutor : IScriptExecutor
    {
        private readonly Func<IConnectionManager> connectionManagerFactory;
        private readonly Func<IUpgradeLog> log;
        private readonly IEnumerable<IScriptPreprocessor> scriptPreprocessors;
        private readonly Func<bool> variablesEnabled;
        
        public int? ExecutionTimeoutSeconds { get; set; }

        public string Schema { get; set; }

        public OracleScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors)
        {
            Schema = schema;
            this.log = log;
            this.variablesEnabled = variablesEnabled;
            this.scriptPreprocessors = scriptPreprocessors;
            this.connectionManagerFactory = connectionManagerFactory;
        }

        public void VerifySchema()
        {
            // Do Nothing
        }


        public void Execute(SqlScript script)
        {
            Execute(script, null);
        }

        public void Execute(SqlScript script, IDictionary<string, string> variables)
        {
            if (variables == null)
                variables = new Dictionary<string, string>();
            
            log().WriteInformation("Executing SQL Server script '{0}'", script.Name);

            var contents = script.Contents;

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
            catch (SqlException sqlException)
            {
                log().WriteInformation("SQL exception has occured in script: '{0}'", script.Name);
                log().WriteError("Script block number: {0}; Block line {1}; Message: {2}", index, sqlException.LineNumber, sqlException.Procedure, sqlException.Number, sqlException.Message);
                log().WriteError(sqlException.ToString());
                throw;
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

        public virtual void Log(IDataReader reader)
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
                    int maxLength = (lines.Count == 0 ? 0 : lines.Max(l => (l[i] ?? "").Length)) + 2;
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

