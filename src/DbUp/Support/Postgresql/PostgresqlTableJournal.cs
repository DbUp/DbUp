using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Support.Postgresql
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// PostgreSQL database using a table called SchemaVersions.
    /// </summary>
    public sealed class PostgresqlTableJournal : IJournal
    {
        private readonly string schema;
        private readonly string table;
        private readonly Func<IConnectionManager> connectionManager;
        private readonly Func<IUpgradeLog> log;

        private static string QuoteIdentifier(string identifier)
        {
            return "\"" + identifier + "\"";
        }

        /// <summary>
        /// Creates a new PostgreSQL table journal.
        /// </summary>
        /// <param name="connectionManager">The PostgreSQL connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="table">The name of the journal table.</param>
        public PostgresqlTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
        {
            this.schema = schema;
            this.table = table;
            this.connectionManager = connectionManager;
            log = logger;
        }

        private static string CreateTableSql(string schema, string table)
        {
            var tableName = CreateTableName(schema, table);
            var primaryKeyName = CreatePrimaryKeyName(schema, table);
            return string.Format(
                            @"CREATE TABLE {0}
                              (
                                schemaversionsid serial NOT NULL,
                                scriptname character varying(255) NOT NULL,
                                applied timestamp without time zone NOT NULL,
                                CONSTRAINT {1} PRIMARY KEY (schemaversionsid)
                              )", tableName, primaryKeyName);
        }

        public string[] GetExecutedScripts()
        {
            log().WriteInformation("Fetching list of already executed scripts.");
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", CreateTableName(schema, table)));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = GetExecutedScriptsSql(schema, table);
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

        /// <summary>
        /// Records an upgrade script for a database.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            var exists = DoesTableExist();
            if (!exists)
            {
                log().WriteInformation(string.Format("Creating the {0} table", CreateTableName(schema, table)));

                connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = CreateTableSql(schema, table);

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }

                    log().WriteInformation(string.Format("The {0} table has been created", CreateTableName(schema, table)));
                });
            }

            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = string.Format("insert into {0} (ScriptName, Applied) values (@scriptName, @applied)", CreateTableName(schema, table));

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

        private static string GetExecutedScriptsSql(string schema, string table)
        {
            var tableName = CreateTableName(schema, table);
            return string.Format("select ScriptName from {0} order by ScriptName", tableName);
        }

        private static string CreateTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema)
                ? QuoteIdentifier(table)
                : QuoteIdentifier(schema) + "." + QuoteIdentifier(table);
        }

        private static string CreatePrimaryKeyName(string schema, string table)
        {
            return QuoteIdentifier("PK_" + table + "_Id");
        }

        private bool DoesTableExist()
        {
            return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = string.Format("select count(*) from {0}", CreateTableName(schema, table));
                        command.CommandType = CommandType.Text;
                        command.ExecuteScalar();
                        return true;
                    }
                }
                // can't catch NpgsqlException here because this project does not depend upon npgsql
                catch (DbException)
                {
                    return false;
                }
            });
        }
    }
}
