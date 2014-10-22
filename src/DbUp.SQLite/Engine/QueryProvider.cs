using System;
using DbUp.Engine.QueryProviders;

namespace DbUp.SQLite.Engine
{
    /// <summary>
    /// Return queries for SQLite
    /// </summary>
    public class QueryProvider : QueryProviderBase
    {
        /// <summary>
        /// New queries container for SQLite
        /// </summary>
        /// <param name="versioningTableName">Name of table which contains versions</param>
        public QueryProvider(string versioningTableName = null)
        {
            if (!String.IsNullOrEmpty(versioningTableName))
                this.VersionTableName = versioningTableName;
        }

        /// <summary>
        /// Sql create strign to create versioning table
        /// </summary>
        /// <returns>Sql command for creating of version table</returns>
        public override string VersionTableCreationString()
        {
            return String.Format(@"CREATE TABLE {0} (
                        VersionID INTEGER CONSTRAINT 'PK_{1}_VersionID' PRIMARY KEY AUTOINCREMENT NOT NULL,
                        ScriptName TEXT NOT NULL,
                        Applied DATETIME NOT NULL,
                        Remark TEXT NULL )", SQLiteObjectParser.QuoteSqlObjectName(TableName), TableName);
        }
        /// <summary>
        /// Sql string for checking if scheme exists and if not create new scheme.
        /// </summary>
        /// <returns></returns>
        public override string CreateSchemeIfNotExists()
        {
            return @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA [{0}]')";
        }

        /// <summary>
        /// Sql for getting which scipt names are in versioning table
        /// </summary>
        /// <returns>Sql command for selecting scirpt names from VersionTableName</returns>
        public override string GetVersionTableExecutedScriptsSql()
        {
            return String.Format("SELECT ScriptName FROM {0} ORDER BY ScriptName", SQLiteObjectParser.QuoteSqlObjectName(TableName));
        }

        /// <summary>
        /// Sql for inserting new entry in VersionTable
        /// </summary>
        /// <returns>Sql command for inserting new entry in versioning table</returns>
        public override string VersionTableNewEntry()
        {
            return String.Format("INSERT INTO {0} (ScriptName, Applied) VALUES (@scriptName, @applied)", SQLiteObjectParser.QuoteSqlObjectName(TableName));
        }

        /// <summary>
        /// Sql for checking if version table exists
        /// </summary>
        /// <returns>SQL Command which checks if version table has any entries.</returns>
        public override string VersionTableDoesTableExist()
        {
            return String.Format("SELECT COUNT(*) FROM {0}", SQLiteObjectParser.QuoteSqlObjectName(TableName));
        }
    }
}
