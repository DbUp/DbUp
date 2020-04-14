using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.Support;

namespace DbUp.SqlServer
{
    /// <summary>
    /// An implementation of the <see cref="Engine.IJournal"/> interface which tracks version numbers for a 
    /// SQL Server database using a table called dbo.SchemaVersions.
    /// </summary>
    public class SqlTableJournal : TableJournal
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
        public SqlTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, Func<IHasher> hasher, string schema, string table)
            : base(connectionManager, logger, new SqlServerObjectParser(), hasher, schema, table)
        {
        }

        protected override string GetInsertJournalEntrySql(string @scriptName, string @applied, string @hash, SqlScript script)
        {
            if (script.SqlScriptOptions.ScriptType == ScriptType.RunOnChange)
            {
                return $"delete from {FqSchemaTableName} where ScriptName = {@scriptName};" +
                       $"insert into {FqSchemaTableName} (ScriptName, Applied, Hash) values ({@scriptName}, {@applied}, {@hash})";
            }

            return $"insert into {FqSchemaTableName} (ScriptName, Applied) values ({@scriptName}, {@applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return string.Format($@"select  [ScriptName], [Hash]
                                   from     {FqSchemaTableName} sv
                                            JOIN
                                            (select max(Applied) as MaxApplied from {FqSchemaTableName} group by [ScriptName]) as svSelf ON sv.Applied = svSelf.MaxApplied
                                   order    by [ScriptName]");
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
$@"create table {FqSchemaTableName} (
    [Id] int identity(1,1) not null constraint {quotedPrimaryKeyName} primary key,
    [ScriptName] nvarchar(255) not null,
    [Applied] datetime not null,
    [Hash] varchar(100) null
)";
        }
    }
}
