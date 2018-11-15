using System;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.AzureSqlDataWarehouse
{
    /// <summary>
    /// An implementation of the <see cref="Engine.IJournal"/> interface which tracks version numbers for an 
    /// Azure SQL Data Warehouse database using a table called dbo.SchemaVersions.
    /// </summary>
    public class AzureSqlDwTableJournal : TableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True", "dbo", "MyVersionTable");
        /// </example>
        public AzureSqlDwTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new AzureSqlDwServerObjectParser(), schema, table)
        {
        }

        protected override string GetInsertJournalEntrySql(string @scriptName, string @applied)
        {
            return $"insert into {FqSchemaTableName} (ScriptName, Applied) values ({@scriptName}, {@applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select [ScriptName] from {FqSchemaTableName} order by [ScriptName]";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return 
$@"create table {FqSchemaTableName} (
    [Id] int identity(1,1) not null,
    [ScriptName] nvarchar(255) not null,
    [Applied] datetime not null
)
WITH (   
    distribution = round_robin,
    clustered index ([Id])
)";
        }
    }
}