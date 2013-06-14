using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SQLite
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// SQLite database using a table called SchemaVersions.
    /// </summary>
    public sealed class SQLiteTableJournal : IJournal
    {
        private readonly string tableName;
        private readonly IConnectionManager connectionManager;
        private readonly IUpgradeLog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableJournal"/> class.
        /// </summary>
        public SQLiteTableJournal(IConnectionManager connectionManager, string table, IUpgradeLog logger)
        {
            tableName = SQLiteObjectParser.QuoteSqlObjectName(table); 
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
                log.WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", tableName));
                return new string[0];
            }

            var scripts = new List<string>();
            connectionManager.ExecuteCommandsWithManagedConnection(commandFactory =>
            {
                using (var command = commandFactory())
                {
                    command.CommandText = string.Format("select [ScriptName] from {0} order by [ScriptName]", tableName);
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
                log.WriteInformation(string.Format("Creating the {0} table", tableName));

                connectionManager.ExecuteCommandsWithManagedConnection(commandFactory =>
                {
                    using (var command = commandFactory())
                    {
                        command.CommandText = string.Format(
                            @"CREATE TABLE {0} (
	SchemaVersionID INTEGER CONSTRAINT 'PK_SchemaVersions_SchemaVersionID' PRIMARY KEY AUTOINCREMENT NOT NULL,
	ScriptName TEXT NOT NULL,
	Applied DATETIME NOT NULL
)", tableName);

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }

                    log.WriteInformation(string.Format("The {0} table has been created", tableName));
                });


                connectionManager.ExecuteCommandsWithManagedConnection(commandFactory =>
                {
                    using (var command = commandFactory())
                    {
                        command.CommandText = string.Format("insert into {0} (ScriptName, Applied) values (@scriptName, '{1}')", tableName, DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"));

                        var param = command.CreateParameter();
                        param.ParameterName = "scriptName";
                        param.Value = script.Name;
                        command.Parameters.Add(param);

                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                });
            }
        }

        private bool DoesTableExist()
        {
            try
            {
                return connectionManager.ExecuteCommandsWithManagedConnection(commandFactory =>
                {
                    using (var command = commandFactory())
                    {
                        command.CommandText = string.Format("select count(*) from {0}", tableName);
                        command.CommandType = CommandType.Text;
                        command.ExecuteScalar();
                        return true;
                    }
                });
            }
            catch (DbException)
            {
                return false;
            }
        }
    }
}
