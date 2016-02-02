using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;

namespace DbUp.Support.SQLite
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// SQLite database using a table called SchemaVersions.
    /// </summary>
    public sealed class SQLiteTableJournal : SqlTableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableJournal"/> class.
        /// </summary>
        public SQLiteTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string table) : 
            base(connectionManager, logger, null, table)
        {}

        /// <summary>Generates an SQL statement that, when exectuted, will create the journal database table.</summary>
        /// <param name="schema">This parameter is ignored as SQLLite doesn't have schemas.</param>
        /// <param name="table">Desired table name</param>
        /// <returns>A <c>CREATE TABLE</c> SQL statement</returns>
        protected override string CreateTableSql(string schema, string table)
        {
            var tableName = CreateTableName(null, table);
            var primaryKeyName = CreatePrimaryKeyName(table);
            return string.Format(
                            @"CREATE TABLE {0} (
	SchemaVersionID INTEGER CONSTRAINT {1} PRIMARY KEY AUTOINCREMENT NOT NULL,
	ScriptName TEXT NOT NULL,
	Applied DATETIME NOT NULL
)", tableName, primaryKeyName);
        }

        /// <summary>Convert the <c>table</c> value into an appropriately-quoted identifier for the journal table's unique primary key.</summary>
        /// <param name="table">Desired table name</param>
        /// <returns>Quoted journal table primary key identifier</returns>
        protected override string CreatePrimaryKeyName(string table)
        {
            return "'PK_" + table + "_SchemaVersionID'";
        }

        /// <summary>Verify, using database-specific queries, if the table exists in the database.</summary>
        /// <param name="command">The <c>IDbCommand</c> to be used for the query</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="schemaName">The schema for the table</param>
        /// <returns>True if table exists, false otherwise</returns>
        protected override bool VerifyTableExistsCommand(IDbCommand command, string tableName, string schemaName)
        {
            command.CommandText = string.Format("SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = '{0}' COLLATE NOCASE", tableName);
            command.CommandType = CommandType.Text;
            var result = (long)command.ExecuteScalar();
            return result == 1;
        }
    }
}
