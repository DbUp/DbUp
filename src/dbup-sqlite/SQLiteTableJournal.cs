using System;
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
$@"CREATE TABLE {FqSchemaTableName} (
    SchemaVersionID INTEGER CONSTRAINT {quotedPrimaryKeyName} PRIMARY KEY AUTOINCREMENT NOT NULL,
    ScriptName TEXT NOT NULL,
    Applied DATETIME NOT NULL
)";
        }

        protected override string DoesTableExistSql()
        {
            return $"SELECT count(name) FROM sqlite_master WHERE type = 'table' AND name = '{UnquotedSchemaTableName}'";
        }
    }
}
