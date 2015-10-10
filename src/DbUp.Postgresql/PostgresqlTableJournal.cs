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
            :base(connectionManager, logger, new PostgresqlObjectParser(), schema, tableName)
        {
        }    

        protected override IDbCommand GetInsertScriptCommand(Func<IDbCommand> dbCommandFactory, SqlScript script)
        {
            var command = dbCommandFactory();
            command.CommandText = string.Format("insert into {0} (ScriptName, Applied) values (@scriptName, @applied)", SchemaTableName);

            var scriptNameParam = command.CreateParameter();
            scriptNameParam.ParameterName = "scriptName";
            scriptNameParam.Value = script.Name;
            command.Parameters.Add(scriptNameParam);

            var appliedParam = command.CreateParameter();
            appliedParam.ParameterName = "applied";
            appliedParam.Value = DateTime.Now;
            command.Parameters.Add(appliedParam);

            command.CommandType = CommandType.Text;
            return command;
        }

        protected override IDbCommand GetSelectExecutedScriptsCommand(Func<IDbCommand> dbCommandFactory, string schemaTableName)
        {
            var command = dbCommandFactory();
            command.CommandText = string.Format("select ScriptName from {0} order by ScriptName", schemaTableName);
            command.CommandType = CommandType.Text;
            return command;
        }

        protected override IDbCommand GetCreateTableCommand(Func<IDbCommand> dbCommandFactory, string schemaTableName)
        {
            var command = dbCommandFactory();
            var primaryKeyName = QuoteSqlObjectName("PK_" + SchemaTableNameWithoutSchema + "_Id");
            command.CommandText = string.Format(
                @"CREATE TABLE {0}
(
    schemaversionsid serial NOT NULL,
    scriptname character varying(255) NOT NULL,
    applied timestamp without time zone NOT NULL,
    CONSTRAINT {1} PRIMARY KEY (schemaversionsid)
)", schemaTableName, primaryKeyName);
            command.CommandType = CommandType.Text;
            return command;
        }
    }
}
