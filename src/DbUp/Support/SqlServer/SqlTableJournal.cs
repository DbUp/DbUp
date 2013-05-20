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
    public sealed class SqlTableJournal : IJournal
    {
        private readonly string schemaTableName;
        private readonly IConnectionManager connectionManager;
        private readonly IUpgradeLog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <param name="logger">The log.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True", "dbo", "MyVersionTable");
        /// </example>
        public SqlTableJournal(IConnectionManager connectionManager, string schema, string table, IUpgradeLog logger)
        {
            schemaTableName = SqlObjectParser.QuoteSqlObjectName(table);
            if (string.IsNullOrEmpty(schema))
                schemaTableName = SqlObjectParser.QuoteSqlObjectName(table);
            else
                schemaTableName = SqlObjectParser.QuoteSqlObjectName(schema) + "." + SqlObjectParser.QuoteSqlObjectName(table);
            this.connectionManager = connectionManager;
            log = logger;
        }

        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns>All executed scripts.</returns>
        public string[] GetExecutedScripts()
        {
            log.WriteInformation("Fetching list of already executed scripts.");
            var exists = DoesTableExist();
            if (!exists)
            {
                log.WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", schemaTableName));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = string.Format("select [ScriptName] from {0} order by [ScriptName]", schemaTableName);
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
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            var exists = DoesTableExist();
            if (!exists)
            {
                log.WriteInformation(string.Format("Creating the {0} table", schemaTableName));

                connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = string.Format(
    @"create table {0} (
	[Id] int identity(1,1) not null constraint PK_SchemaVersions_Id primary key,
	[ScriptName] nvarchar(255) not null,
	[Applied] datetime not null
)", schemaTableName);

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }

                    log.WriteInformation(string.Format("The {0} table has been created", schemaTableName));
                });
            }

            connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = string.Format("insert into {0} (ScriptName, Applied) values (@scriptName, '{1}')", schemaTableName, DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"));

                    var param = command.CreateParameter();
                    param.ParameterName = "scriptName";
                    param.Value = script.Name;
                    command.Parameters.Add(param);

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            });
        }

        private bool DoesTableExist()
        {
            return connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = string.Format("select count(*) from {0}", schemaTableName);
                        command.CommandType = CommandType.Text;
                        command.ExecuteScalar();
                        return true;
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
    }
}