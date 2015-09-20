using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.Firebird
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// Firebird database using a table called SchemaVersions.
    /// </summary>
    public sealed class FirebirdTableJournal : TableJournal
    {       

        /// <summary>
        /// Creates a new Firebird table journal.
        /// </summary>
        /// <param name="connectionManager">The Firebird connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="tableName">The name of the journal table.</param>
        public FirebirdTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string tableName)
            :base(connectionManager, logger, null, tableName)
        {
          
        }

        private static string GetCreateTableSql(string tableName)
        {
            return string.Format(@"CREATE TABLE {0}
                                    (
                                        schemaversionsid INTEGER NOT NULL,
                                        scriptname VARCHAR(255) NOT NULL,
                                        applied TIMESTAMP NOT NULL,
                                        CONSTRAINT pk_{0}_id PRIMARY KEY (schemaversionsid)
                                    )", tableName);
        }

        private static string CreateGeneratorSql(string tableName)
        {
            return string.Format(@"CREATE SEQUENCE {0}", GeneratorName(tableName));
        }

        private static string CreateTriggerSql(string tableName)
        {
            return string.Format(
                                @"CREATE TRIGGER {0} FOR {1} ACTIVE BEFORE INSERT POSITION 0 AS BEGIN
                                        if (new.schemaversionsid is null or (new.schemaversionsid = 0)) then new.schemaversionsid = gen_id({2},1);
                                  END;", TriggerName(tableName), tableName, GeneratorName(tableName));
        }                     

        private static string GeneratorName(string tableName)
        {
            return string.Format("GEN_{0}ID", tableName);
        }

        private static string TriggerName(string tableName)
        {
            return string.Format("BI_{0}ID", tableName);
        }

        private static string GetExecutedScriptsSql(string table)
        {
            return string.Format("select ScriptName from {0} order by ScriptName", table);
        }

        private void ExecuteCommand(Func<IDbCommand> dbCommandFactory, string sql)
        {
            using (var command = dbCommandFactory())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        protected override void OnTableCreated(Func<IDbCommand> dbCommandFactory)
        {
            ExecuteCommand(dbCommandFactory, CreateGeneratorSql(SchemaTableName));
            Log().WriteInformation(string.Format("The {0} generator has been created", GeneratorName(SchemaTableName)));
            ExecuteCommand(dbCommandFactory, CreateTriggerSql(SchemaTableName));
            Log().WriteInformation(string.Format("The {0} trigger has been created", TriggerName(SchemaTableName)));
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
