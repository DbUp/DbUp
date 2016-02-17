using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// SQL Server database using a table called dbo.SchemaVersions.
    /// </summary>
    public class SqlTableJournal : IJournal
    {
        private readonly Func<IConnectionManager> connectionManager;
        private readonly Func<IUpgradeLog> log;
        private readonly string schema;
        private readonly string table;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True", "dbo", "MyVersionTable");
        /// </example>
        public SqlTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
        {
            this.schema = schema;
            this.table = table;

            this.connectionManager = connectionManager;

            log = logger;
        }

        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns>All executed scripts.</returns>
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
        /// Create an SQL statement which will retrieve all executed scripts in order.
        /// </summary>
        protected virtual string GetExecutedScriptsSql(string schema, string table)
        {
            return string.Format("select [ScriptName] from {0} order by [ScriptName]", CreateTableName(schema, table));
        }

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
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

        /// <summary>Generates an SQL statement that, when exectuted, will create the journal database table.</summary>
        /// <param name="schema">Desired schema name supplied by configuration or <c>NULL</c></param>
        /// <param name="table">Desired table name</param>
        /// <returns>A <c>CREATE TABLE</c> SQL statement</returns>
        protected virtual string CreateTableSql(string schema, string table)
        {
            var tableName = CreateTableName(schema, table);
            var primaryKeyConstraintName = CreatePrimaryKeyName(table);

            return string.Format(@"create table {0} (
	[Id] int identity(1,1) not null constraint {1} primary key,
	[ScriptName] nvarchar(255) not null,
	[Applied] datetime not null
)", tableName, primaryKeyConstraintName);
        }

        /// <summary>Combine the <c>schema</c> and <c>table</c> values into an appropriately-quoted identifier for the journal table.</summary>
        /// <param name="schema">Desired schema name supplied by configuration or <c>NULL</c></param>
        /// <param name="table">Desired table name</param>
        /// <returns>Quoted journal table identifier</returns>
        protected virtual string CreateTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema)
                ? SqlObjectParser.QuoteSqlObjectName(table)
                : SqlObjectParser.QuoteSqlObjectName(schema) + "." + SqlObjectParser.QuoteSqlObjectName(table);
        }

        /// <summary>Convert the <c>table</c> value into an appropriately-quoted identifier for the journal table's unique primary key.</summary>
        /// <param name="table">Desired table name</param>
        /// <returns>Quoted journal table primary key identifier</returns>
        protected virtual string CreatePrimaryKeyName(string table)
        {
            return SqlObjectParser.QuoteSqlObjectName("PK_" + table + "_Id");
        }

        private bool DoesTableExist()
        {
            return connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        return VerifyTableExistsCommand(command, table, schema);
                    }
                }
                catch (SqlException)
                {
                    return false;
                }
                catch (DbException)
                {
                    return false;
                }
            });
        }

        /// <summary>Verify, using database-specific queries, if the table exists in the database.</summary>
        /// <param name="command">The <c>IDbCommand</c> to be used for the query</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="schemaName">The schema for the table</param>
        /// <returns>True if table exists, false otherwise</returns>
        protected virtual bool VerifyTableExistsCommand(IDbCommand command, string tableName, string schemaName)
        {
            command.CommandText = string.IsNullOrEmpty(schema)
                            ? string.Format("select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}'", tableName)
                            : string.Format("select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}' and TABLE_SCHEMA = '{1}'", tableName, schemaName);
            command.CommandType = CommandType.Text;
            var result = command.ExecuteScalar() as int?;
            return result == 1;
        }
    }
}