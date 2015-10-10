using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DbUp.Support
{
    /// <summary>
    /// The base class for Journal implementations that use a table.
    /// </summary>
    public abstract class TableJournal : IJournal
    {
        private bool tableExists = false;
        private bool tableIsLatestVersion = false;     
        private bool tableRequiresCreation = false;

        private ISqlObjectParser sqlObjectParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>        
        public TableJournal(Func<IConnectionManager> connectionManager, 
                            Func<IUpgradeLog> logger, ISqlObjectParser sqlObjectParser, string schema, string table)
        {
            ConnectionManager = connectionManager;
            Log = logger;
            this.sqlObjectParser = sqlObjectParser;
            SchemaTableName = string.IsNullOrEmpty(schema)
                ? QuoteSqlObjectName(table)
                : QuoteSqlObjectName(schema) + "." + QuoteSqlObjectName(table);
        }

        protected string SchemaTableName { get; private set; }

        protected Func<IConnectionManager> ConnectionManager { get; set; }

        protected Func<IUpgradeLog> Log { get; set; }

        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns>All executed scripts.</returns>
        public string[] GetExecutedScripts()
        {
            Log().WriteInformation("Fetching list of already executed scripts.");            
            if (!TableExists)
            {                
                Log().WriteInformation(string.Format("The {0} table could not be found. The database is assumed to be at version 0.", SchemaTableName));
                return new string[0];
            }

            // Ensure the table is migrated to the latest version before we use it.
            EnsureTableIsLatestVersion();

            var scripts = new List<string>();
            ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = GetSelectExecutedScriptsCommand(dbCommandFactory, SchemaTableName))
                {
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
            if (!TableExists)
            {
                // Ensure the table is migrated to the latest version before we use it.
                EnsureTableIsLatestVersion();
            }

            ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = GetInsertScriptCommand(dbCommandFactory, script))
                {
                    command.ExecuteNonQuery();
                }
            });
        }

        protected abstract IDbCommand GetInsertScriptCommand(Func<IDbCommand> dbCommandFactory, SqlScript script);

        protected abstract IDbCommand GetSelectExecutedScriptsCommand(Func<IDbCommand> dbCommandFactory, string schemaTableName);

        protected abstract IDbCommand GetCreateTableCommand(Func<IDbCommand> dbCommandFactory, string schemaTableName);

        /// <summary>
        /// Quotes the name of the SQL object in square brackets to allow Special characters in the object name.
        /// This function implements System.Data.SqlClient.SqlCommandBuilder.QuoteIdentifier() with an additional
        /// validation which is missing from the SqlCommandBuilder version.
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        protected virtual string QuoteSqlObjectName(string objectName)
        {                   
            return sqlObjectParser.QuoteIdentifier(objectName);
        }

        /// <summary>
        /// Unquotes a quoted identifier.       
        /// </summary>
        /// <param name="objectName">identifier to unquote.</param>    
        protected virtual string UnquoteSqlObjectName(string quotedIdentifier)
        {
            return sqlObjectParser.UnquoteIdentifier(quotedIdentifier);
        }

        /// <summary>
        /// The sql to select a scalar value from a table. Typically used to check whether a table exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual string GetSelectScalarFromTableSql(string tableName)
        {
            return string.Format("select count(*) from {0}", tableName);
        }

        protected virtual void EnsureTableIsLatestVersion()
        {
            //TODO: We don't currnelty do any kind of migration, we only create the table if it doesn't exist.
            // This may change if we find a way to support migrations for the journal table in the future!
            // The thinking is the table would allways be created first in code in its old / original schema,
            // but then migrations would be run on it to bring it up to the latest state.        
            if (!tableIsLatestVersion)
            {                
                if (!TableExists)
                {
                    Log().WriteInformation(string.Format("Creating the {0} table", SchemaTableName));
                    ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                    {
                        // We will never change the schema of the initial table create.
                        using (var command = GetCreateTableCommand(dbCommandFactory, SchemaTableName))
                        {
                            command.ExecuteNonQuery();
                        }
                        Log().WriteInformation(string.Format("The {0} table has been created", SchemaTableName));

                        OnTableCreated(dbCommandFactory);
                    });

                    
                }
                tableIsLatestVersion = true;
            }
        }

        protected virtual void OnTableCreated(Func<IDbCommand> dbCommandFactory)
        {
            // TODO: Now we could run any migration scripts on it using some mechanism to make sure the table is ready for use.
            
        }

        protected bool TableExists
        {
            get
            {
                // We cache when it does exist so we don't check multiple times unnecessarily.
                tableExists = tableExists || DoesTableExist();
                return tableExists;
            }
        }

        protected virtual bool DoesTableExist()
        {
            if(tableRequiresCreation)
            {
                // this means we have previously checked and ascertained that the table needs to be created.
                return false;
            }
            Log().WriteInformation("Checking whether journal table exists..");
            return ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                try
                {
                    using (var command = dbCommandFactory())
                    {
                        command.CommandText = GetSelectScalarFromTableSql(SchemaTableName);
                        command.CommandType = CommandType.Text;
                        command.ExecuteScalar();
                        return true;
                    }
                }
                catch (DbException)
                {
                    tableRequiresCreation = true;
                    return false;
                }
            });
        }
    }
}
