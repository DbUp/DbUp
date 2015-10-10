using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.SQLite
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// SQLite database using a table called SchemaVersions.
    /// </summary>
    public class SQLiteTableJournal : TableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableJournal"/> class.
        /// </summary>
        public SQLiteTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string table) :
            base(connectionManager, logger, new SQLiteObjectParser(), null, table)
        { }

        /// <summary>
        /// Create table sql for SQLite
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetCreateTableSql(string tableName)
        {
            return string.Format(
                            @"CREATE TABLE {0} (
	SchemaVersionID INTEGER CONSTRAINT 'PK_SchemaVersions_SchemaVersionID' PRIMARY KEY AUTOINCREMENT NOT NULL,
	ScriptName TEXT NOT NULL,
	Applied DATETIME NOT NULL
)", tableName);
        }

        /// <summary>
        /// The Sql which is used to return the names of the scripts which have allready been executed from the journal table. 
        /// </summary>
        private string GetExecutedScriptsSql(string table)
        {
            return string.Format("select [ScriptName] from {0} order by [ScriptName]", table);
        }

        protected override IDbCommand GetCreateTableCommand(Func<IDbCommand> dbCommandFactory, string schemaTableName)
        {
            var command = dbCommandFactory();
            command.CommandText = GetCreateTableSql(SchemaTableName);
            command.CommandType = CommandType.Text;
            return command;
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
            command.CommandText = GetExecutedScriptsSql(schemaTableName);
            command.CommandType = CommandType.Text;
            return command;
        }
    }
}
