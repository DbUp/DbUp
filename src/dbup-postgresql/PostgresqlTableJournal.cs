using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.Postgresql
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a
    /// PostgreSQL database using a table called SchemaVersions.
    /// </summary>
    public class PostgresqlTableJournal : TableJournal
    {
        /// <summary>
        /// Creates a new PostgreSQL table journal.
        /// </summary>
        /// <param name="connectionManager">The PostgreSQL connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="tableName">The name of the journal table.</param>
        public PostgresqlTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string tableName)
            : base(connectionManager, logger, new PostgresqlObjectParser(), schema, tableName)
        {
        }

        protected override IDbCommand GetInsertScriptCommand(Func<IDbCommand> dbCommandFactory, SqlScript script)
        {
            // EnableSqlRewriting is enabled by default, and needs to be explicitly disabled
            bool enableSqlRewriting = !AppContext.TryGetSwitch("Npgsql.EnableSqlRewriting", out bool enabled) || enabled;

            if (enableSqlRewriting)
                return base.GetInsertScriptCommand(dbCommandFactory, script);

            // Use positional parameters instead of named parameters
            var command = dbCommandFactory();

            var scriptNameParam = command.CreateParameter();
            scriptNameParam.Value = script.Name;
            command.Parameters.Add(scriptNameParam);

            var appliedParam = command.CreateParameter();
            appliedParam.Value = DateTime.Now;
            command.Parameters.Add(appliedParam);

            command.CommandText = GetInsertJournalEntrySql("$1", "$2");
            command.CommandType = CommandType.Text;
            return command;
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string applied)
        {
            return $"insert into {FqSchemaTableName} (ScriptName, Applied) values ({scriptName}, {applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select ScriptName from {FqSchemaTableName} order by ScriptName";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
$@"CREATE TABLE {FqSchemaTableName}
(
    schemaversionsid serial NOT NULL,
    scriptname character varying(255) NOT NULL,
    applied timestamp without time zone NOT NULL,
    CONSTRAINT {quotedPrimaryKeyName} PRIMARY KEY (schemaversionsid)
)";
        }
    }
}
