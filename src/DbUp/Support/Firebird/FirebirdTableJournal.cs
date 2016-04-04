using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DbUp.Support.Firebird
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// Firebird database using a table called SchemaVersions.
    /// </summary>
    public sealed class FirebirdTableJournal : IJournal
    {
        private readonly Func<IConnectionManager> connectionManager;
        private readonly Func<IUpgradeLog> log;
        private readonly string tableName;

        /// <summary>
        /// Creates a new Firebird table journal.
        /// </summary>
        /// <param name="connectionManager">The Firebird connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="tableName">The name of the journal table.</param>
        public FirebirdTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string tableName)
        {
            log = logger;
            this.tableName = tableName;
            this.connectionManager = connectionManager;
        }

        private static string CreateTableSql(string tableName)
        {
            return string.Format(@"CREATE TABLE {0}
                                    (
                                        schemaversionsid INTEGER NOT NULL,
                                        scriptname VARCHAR(255) NOT NULL,
                                        applied TIMESTAMP NOT NULL,
                                        CONSTRAINT pk_{0}_id PRIMARY KEY (schemaversionsid)
                                    )", tableName);
        }

        private static string CreateGeneratorSql(string tableName)
        {
            return string.Format(@"CREATE SEQUENCE {0}", GeneratorName(tableName));
        }

        private static string CreateTriggerSql(string tableName)
        {
            return string.Format(
                                @"CREATE TRIGGER {0} FOR {1} ACTIVE BEFORE INSERT POSITION 0 AS BEGIN
                                        if (new.schemaversionsid is null or (new.schemaversionsid = 0)) then new.schemaversionsid = gen_id({2},1);
                                  END;", TriggerName(tableName), tableName, GeneratorName(tableName));
        }

        /// <summary>
        /// Fetches the list of already executed scripts.
        /// </summary>
        /// <returns></returns>
        public string[] GetExecutedScripts()
        {
            log().WriteInformation("Fetching list of already executed scripts.");
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", tableName));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = GetExecutedScriptsSql(tableName);
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            scripts.Add((string)reader[0]);
                    }
                }
            });

            return scripts.ToArray();
        }

        private void ExecuteCommand(Func<IDbCommand> dbCommandFactory, string sql)
        {
            using (var command = dbCommandFactory())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        private static string GeneratorName(string tableName)
        {
            return string.Format("GEN_{0}ID", tableName);
        }

        private static string TriggerName(string tableName)
        {
            return string.Format("BI_{0}ID", tableName);
        }

        /// <summary>
        /// Records an upgrade script for a database.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("Creating the {0} table", tableName));

                connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    ExecuteCommand(dbCommandFactory, CreateTableSql(tableName));
                    log().WriteInformation(string.Format("The {0} table has been created", tableName));
                    ExecuteCommand(dbCommandFactory, CreateGeneratorSql(tableName));
                    log().WriteInformation(string.Format("The {0} generator has been created", GeneratorName(tableName)));
                    ExecuteCommand(dbCommandFactory, CreateTriggerSql(tableName));
                    log().WriteInformation(string.Format("The {0} trigger has been created", TriggerName(tableName)));
                });
            }

            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = string.Format("insert into {0} (ScriptName, Applied) values (@scriptName, @applied)", tableName);

                    var scriptNameParam = command.CreateParameter();
                    scriptNameParam.ParameterName = "scriptName";
                    scriptNameParam.Value = script.Name;
                    command.Parameters.Add(scriptNameParam);

                    var appliedParam = command.CreateParameter();
                    appliedParam.ParameterName = "applied";
                    appliedParam.Value = DateTime.Now;
                    command.Parameters.Add(appliedParam);

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            });
        }

        private static string GetExecutedScriptsSql(string table)
        {
            return string.Format("select ScriptName from {0} order by ScriptName", table);
        }

        private bool DoesTableExist()
        {
            return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        return VerifyTableExistsCommand(command);
                    }
                }
                // can't catch FbException here because this project does not depend upon Firebird
                catch (DbException)
                {
                    return false;
                }
            });
        }

        /// <summary>Verify, using database-specific queries, if the table exists in the database.</summary>
        /// <param name="command">The <c>IDbCommand</c> to be used for the query</param>
        /// <returns>True if table exists, false otherwise</returns>
        private bool VerifyTableExistsCommand(IDbCommand command)
        {
            command.CommandText = string.Format("select 1 from RDB$RELATIONS where RDB$SYSTEM_FLAG = 0 and RDB$RELATION_NAME = '{0}'", tableName);
            command.CommandType = CommandType.Text;
            var result = command.ExecuteScalar() as int?;
            return result == 1;
        }
    }
}
