using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DbUp.Engine;
using DbUp.Engine.Output;

namespace DbUp.Support.Oracle
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// SQL Server database using a table called dbo.SchemaVersions.
    /// </summary>
    public sealed class OracleTableJournal : IJournal
    {
        /// <summary>
        /// The default schema for the journal table
        /// </summary>
        public const string DefaultSchema = "DBUP";

        /// <summary>
        /// The default journal table name
        /// </summary>
        public const string DefaultTableName = "SCHEMA_VERSIONS";

        private readonly Func<IDbConnection> connectionFactory;
        private readonly string schemaTableName;
        private readonly IUpgradeLog log;
        private readonly string table;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbUp.Support.Oracle.OracleTableJournal"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <param name="logger">The log.</param>
        public OracleTableJournal(Func<IDbConnection> connectionFactory, string schema, string table, IUpgradeLog logger)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException("connectionFactory");
            if (schema == null)
                throw new ArgumentNullException("schema");
            if (table == null)
                throw new ArgumentNullException("table");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.connectionFactory = connectionFactory;
            this.table = table;
            this.schemaTableName = schema + "." + table;
            this.log = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbUp.Support.Oracle.OracleTableJournal"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The log.</param>
        public OracleTableJournal(Func<IDbConnection> connectionFactory, IUpgradeLog logger)
            : this(connectionFactory, DefaultSchema, DefaultTableName, logger)
        {
        }

        /// <summary>
        /// Retrieves executed scripts from the database
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
            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT SCRIPT_NAME FROM {0} ORDER BY SCRIPT_NAME", schemaTableName);
                command.CommandType = CommandType.Text;
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        scripts.Add((string)reader[0]);
                }
            }
            return scripts.ToArray();
        }

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            const string sql = "CREATE TABLE {0} (ID VARCHAR2(32) DEFAULT sys_guid() NOT NULL, SCRIPT_NAME VARCHAR2(255) NOT NULL, APPLIED DATE NOT NULL, CONSTRAINT PK_{1} PRIMARY KEY (ID) ENABLE VALIDATE)";

            var exists = DoesTableExist();
            if (!exists)
            {
                log.WriteInformation(string.Format("Creating the {0} table", schemaTableName));

                using (var connection = connectionFactory())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(sql, schemaTableName, table);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                log.WriteInformation(string.Format("The {0} table has been created", schemaTableName));
            }

            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = string.Format("INSERT INTO {0} (SCRIPT_NAME, APPLIED) VALUES ('{1}', TO_DATE('{2:yyyy-MM-dd hh:mm:ss}', 'yyyy-mm-dd hh24:mi:ss'))", schemaTableName, script.Name, DateTime.UtcNow);
                command.CommandType = CommandType.Text;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private bool DoesTableExist()
        {
            try
            {
                using (var connection = connectionFactory())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format("SELECT COUNT(*) FROM {0}", schemaTableName);
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteScalar();
                        return true;
                    }
                }
            }
            catch (DbException)
            {
                return false;
            }
        }
    }
}