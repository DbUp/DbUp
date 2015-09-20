using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using DbUp.Support.SqlServer;

namespace DbUp.SqlCe
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// Sql Server Ce database using a table called dbo.SchemaVersions.
    /// </summary>
    public class SqlCeTableJournal : TableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeTableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True", "dbo", "MyVersionTable");
        /// </example>
        public SqlCeTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, schema, table)
        {

        }

        protected override string QuoteSqlObjectName(string objectName)
        {
            return SqlObjectParser.QuoteSqlObjectName(objectName, ObjectNameOptions.Trim);
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
        
        /// <summary>
        /// The Sql which is used to return the names of the scripts which have allready been executed from the journal table. 
        /// </summary>
        protected virtual string GetExecutedScriptsSql(string table)
        {
            return string.Format("select [ScriptName] from {0} order by [ScriptName]", table);
        }

        /// <summary>
        /// The sql to exectute to create the schema versions table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual string GetCreateTableSql(string tableName)
        {
            return string.Format(@"create table {0} (
        	[Id] int identity(1,1) not null constraint PK_SchemaVersions_Id primary key,
        	[ScriptName] nvarchar(255) not null,
        	[Applied] datetime not null
        )", tableName);
        }
    }
}