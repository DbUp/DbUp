using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.Support;

namespace DbUp.Redshift
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a
    /// Redshift database using a table called SchemaVersions.
    /// </summary>
    public class RedshiftTableJournal : TableJournal
    {
        /// <summary>
        /// Creates a new Redshift table journal.
        /// </summary>
        /// <param name="connectionManager">The Redshift connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="tableName">The name of the journal table.</param>
        public RedshiftTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, Func<IHasher> hasher, string schema, string tableName)
            : base(connectionManager, logger, new RedshiftObjectParser(), hasher, schema, tableName)
        {
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string applied, string hash, SqlScript script)
        {
           return $"insert into {FqSchemaTableName} (ScriptName, Applied) values ({@scriptName}, {@applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select ScriptName from {FqSchemaTableName} order by ScriptName";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
$@"CREATE TABLE {FqSchemaTableName}
(
    schemaversionsid BIGINT IDENTITY NOT NULL,
    scriptname character varying(255) NOT NULL,
    applied timestamp without time zone NOT NULL,
    CONSTRAINT {quotedPrimaryKeyName} PRIMARY KEY (schemaversionsid)
)";
        }
    }
}
