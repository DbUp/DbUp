using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;

namespace DbUp.Support.AzureSqlDataWarehouse
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// Azure Sql Data Warehouse database using a table called SchemaVersions.
    /// </summary>
    public sealed class AzureSqlDataWarehouseTableJournal : SqlTableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSqlDataWarehouseTableJournal"/> class.
        /// </summary>
        public AzureSqlDataWarehouseTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string table) :
            base(connectionManager, logger, null, table)
        { }

        /// <summary>Generates an SQL statement that, when exectuted, will create the journal database table.</summary>
        /// <param name="schema">This parameter is ignored as SQLLite doesn't have schemas.</param>
        /// <param name="table">Desired table name</param>
        /// <returns>A <c>CREATE TABLE</c> SQL statement</returns>
        protected override string CreateTableSql(string schema, string table)
        {
            var tableName = CreateTableName(null, table);
            return string.Format(
@"CREATE TABLE {0}
(
	[Id] uniqueidentifier not null,
	[ScriptName] nvarchar(255) not null,
	[Applied] datetime not null
)
WITH 
(	
     DISTRIBUTION = ROUND_ROBIN,
     CLUSTERED COLUMNSTORE INDEX  
)
", tableName);
        }

        /// <summary>
        /// Inserts a script into the SchemaVersion table. 
        /// </summary>
        /// <param name="connectionManager">The connection manager used to run the command.</param>
        /// <param name="schema">Desired schema name supplied by configuration or <c>NULL</c>.</param>
        /// <param name="table">Desired table name<./param>
        /// <param name="script">The script to insert.</param>
        protected override void ExecuteInsertScriptAction(Func<IConnectionManager> connectionManager, string schema, string table, SqlScript script)
        {
            var tableName = CreateTableName(null, table);

            connectionManager().ExecuteCommandsWithManagedConnection(dbCommandFactory =>
            {
                using (var command = dbCommandFactory())
                {
                    command.CommandText = string.Format("INSERT INTO {0} VALUES(@nextId, @scriptName, @applied)", tableName);

                    // Since Azure SQL Data Warehouse does not support auto incrementing columns,  generate a unqiue identifier for each row.
                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "nextId";
                    idParam.Value = Guid.NewGuid().ToString();
                    command.Parameters.Add(idParam);

                    var scriptNameParam = command.CreateParameter();
                    scriptNameParam.ParameterName = "scriptName";
                    scriptNameParam.DbType = DbType.String;
                    scriptNameParam.Value = script.Name;
                    command.Parameters.Add(scriptNameParam);

                    var appliedParam = command.CreateParameter();
                    appliedParam.ParameterName = "applied";
                    appliedParam.Value = DateTime.UtcNow;
                    command.Parameters.Add(appliedParam);

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            });
        }
    }
}
