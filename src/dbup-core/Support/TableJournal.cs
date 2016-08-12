﻿using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

// ReSharper disable MemberCanBePrivate.Global
namespace DbUp.Support
{
    /// <summary>
    /// The base class for Journal implementations that use a table.
    /// </summary>
    public abstract class TableJournal : IJournal
    {
        readonly ISqlObjectParser sqlObjectParser;
        bool journalExists;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="sqlObjectParser"></param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        protected TableJournal(
            Func<IConnectionManager> connectionManager,
            Func<IUpgradeLog> logger,
            ISqlObjectParser sqlObjectParser,
            string schema, string table)
        {
            this.sqlObjectParser = sqlObjectParser;
            ConnectionManager = connectionManager;
            Log = logger;
            UnquotedSchemaTableName = table;
            SchemaTableSchema = schema;
            FqSchemaTableName = string.IsNullOrEmpty(schema)
                ? sqlObjectParser.QuoteIdentifier(table)
                : sqlObjectParser.QuoteIdentifier(schema) + "." + sqlObjectParser.QuoteIdentifier(table);
        }

        protected string SchemaTableSchema { get; private set; }

        /// <summary>
        /// Schema table name, no schema and unquoted
        /// </summary>
        protected string UnquotedSchemaTableName { get; private set; }

        /// <summary>
        /// Fully qualified schema table name, includes schema and is quoted.
        /// </summary>
        protected string FqSchemaTableName { get; private set; }

        protected Func<IConnectionManager> ConnectionManager { get; private set; }

        protected Func<IUpgradeLog> Log { get; private set; }

        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns>All executed scripts.</returns>
        public string[] GetExecutedScripts()
        {
            EnsureTableIsLatestVersion();
            Log().WriteInformation("Fetching list of already executed scripts.");

            var scripts = new List<string>();
            ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = GetJournalEntriesCommand(dbCommandFactory))
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
        /// <param name="dbCommandFactory"></param>
        public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
        {
            EnsureTableIsLatestVersion();
            using (var command = GetInsertScriptCommand(dbCommandFactory, script))
            {
                command.ExecuteNonQuery();
            }
        }

        protected IDbCommand GetInsertScriptCommand(Func<IDbCommand> dbCommandFactory, SqlScript script)
        {
            var command = dbCommandFactory();

            var scriptNameParam = command.CreateParameter();
            scriptNameParam.ParameterName = "scriptName";
            scriptNameParam.Value = script.Name;
            command.Parameters.Add(scriptNameParam);

            var appliedParam = command.CreateParameter();
            appliedParam.ParameterName = "applied";
            appliedParam.Value = DateTime.Now;
            command.Parameters.Add(appliedParam);

            command.CommandText = GetInsertJournalEntrySql("@scriptName", "@applied");
            command.CommandType = CommandType.Text;
            return command;
        }

        protected IDbCommand GetJournalEntriesCommand(Func<IDbCommand> dbCommandFactory)
        {
            var command = dbCommandFactory();
            command.CommandText = GetJournalEntriesSql();
            command.CommandType = CommandType.Text;
            return command;
        }

        protected IDbCommand GetCreateTableCommand(Func<IDbCommand> dbCommandFactory)
        {
            var command = dbCommandFactory();
            var primaryKeyName = sqlObjectParser.QuoteIdentifier("PK_" + UnquotedSchemaTableName + "_Id");
            command.CommandText = CreateSchemaTableSql(primaryKeyName);
            command.CommandType = CommandType.Text;
            return command;
        }

        /// <summary>
        /// Sql for inserting a journal entry
        /// </summary>
        /// <param name="scriptName">Name of the script name param (i.e @scriptName)</param>
        /// <param name="applied">Name of the applied param (i.e @applied)</param>
        /// <returns></returns>
        protected abstract string GetInsertJournalEntrySql(string @scriptName, string @applied);

        /// <summary>
        /// Sql for getting the journal entries
        /// </summary>
        protected abstract string GetJournalEntriesSql();

        /// <summary>
        /// Sql for creating journal table
        /// </summary>
        /// <param name="quotedPrimaryKeyName">Following PK_{TableName}_Id naming</param>
        protected abstract string CreateSchemaTableSql(string quotedPrimaryKeyName);

        /// <summary>
        /// Unquotes a quoted identifier.
        /// </summary>
        /// <param name="objectName">identifier to unquote.</param>
        protected string UnquoteSqlObjectName(string quotedIdentifier)
        {
            return sqlObjectParser.UnquoteIdentifier(quotedIdentifier);
        }

        protected virtual void OnTableCreated(Func<IDbCommand> dbCommandFactory)
        {
            // TODO: Now we could run any migration scripts on it using some mechanism to make sure the table is ready for use.
            
        }

        protected void EnsureTableIsLatestVersion()
        {
            if (!journalExists && !DoesTableExist())
            {
                Log().WriteInformation(string.Format("Creating the {0} table", FqSchemaTableName));
                ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
                {
                    // We will never change the schema of the initial table create.
                    using (var command = GetCreateTableCommand(dbCommandFactory))
                    {
                        command.ExecuteNonQuery();
                    }
                    Log().WriteInformation(string.Format("The {0} table has been created", FqSchemaTableName));

                    OnTableCreated(dbCommandFactory);
                });
            }

            journalExists = true;
        }

        protected bool DoesTableExist()
        {
            Log().WriteInformation("Checking whether journal table exists..");
            return ConnectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = DoesTableExistSql();
                    command.CommandType = CommandType.Text;
                    var executeScalar = command.ExecuteScalar();
                    if (executeScalar == null)
                        return false;
                    if (executeScalar is long)
                        return (long)executeScalar == 1;
                    return (int)executeScalar == 1;
                }
            });
        }

        /// <summary>Verify, using database-specific queries, if the table exists in the database.</summary>
        /// <returns>1 if table exists, 0 otherwise</returns>
        protected virtual string DoesTableExistSql()
        {
            return string.IsNullOrEmpty(SchemaTableSchema)
                            ? string.Format("select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}'", UnquotedSchemaTableName)
                            : string.Format("select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}' and TABLE_SCHEMA = '{1}'", UnquotedSchemaTableName, SchemaTableSchema);
        }
    }
}
