using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

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
            : base(connectionManager, logger, new SqlCeObjectParser(), schema, table)
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
    [Id] int identity(1,1) not null constraint {quotedPrimaryKeyName} primary key,
    [ScriptName] nvarchar(255) not null,
    [Applied] datetime not null
)";
        }

        protected override string DoesTableExistSql()
        {
            if (string.IsNullOrEmpty(SchemaTableSchema))
                return $"SELECT count(*) FROM information_schema.tables WHERE table_name='{UnquotedSchemaTableName}'";

            return $"SELECT count(*) FROM information_schema.tables WHERE table_schema = '{SchemaTableSchema}'AND table_name = '{UnquotedSchemaTableName}')";

        }
    }
}