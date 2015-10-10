using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.MySql
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// MySql database using a table called SchemaVersions.
    /// </summary>
    public class MySqlTableJournal : TableJournal
    {              
        /// <summary>
        /// Creates a new MySql table journal.
        /// </summary>
        /// <param name="connectionManager">The MySql connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="table">The name of the journal table.</param>
        public MySqlTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            :base(connectionManager, logger, new MySqlObjectParser(), schema, table)
        {           
        }        

        private static string GetCreateTableSql(string tableName)
        {
            return string.Format(
                @"CREATE TABLE {0} 
                    (
                        `schemaversionid` INT NOT NULL AUTO_INCREMENT,
                        `scriptname` VARCHAR(255) NOT NULL,
                        `applied` TIMESTAMP NOT NULL,
                        PRIMARY KEY (`schemaversionid`));", tableName);
        }               

        private static string GetExecutedScriptsSql(string table)
        {
            return string.Format("select scriptname from {0} order by scriptname", table);
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

        protected override IDbCommand GetCreateTableCommand(Func<IDbCommand> dbCommandFactory, string schemaTableName)
        {
            var command = dbCommandFactory();
            command.CommandText = GetCreateTableSql(SchemaTableName);
            command.CommandType = CommandType.Text;
            return command;
        }
    }
}
