using System;
using System.Globalization;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.SqlAnywhere
{
    /// <summary>
    /// An implementation of the <see cref="Engine.IJournal"/> interface which tracks version numbers for a
    /// Sql Anywhere database using a table called SchemaVersions.
    /// </summary>
    public class SqlAnywhereTableJournal : TableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAnywhereTableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True", "dba", "MyVersionTable");
        /// </example>
        public SqlAnywhereTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new SqlAnywhereObjectParser(), schema, table)
        {
        }

        public static CultureInfo English = new CultureInfo("en-UK", false);

        protected override string GetInsertJournalEntrySql(string @scriptName, string @applied)
        {
            return $"insert into {FqSchemaTableName} (ScriptName, Applied) values (? ,?)";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select [ScriptName] from {FqSchemaTableName} order by [ScriptName]";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
                $@"create table {FqSchemaTableName} (
                    [Id] int identity not null constraint {quotedPrimaryKeyName} primary key,
                    [ScriptName] nvarchar(255) not null,
                    [Applied] datetime not null
                )";
        }

        protected override string DoesTableExistSql()
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"select cast(1 as Int) from systab t where t.table_name = '{unquotedSchemaTableName}'";
        }
    }
}