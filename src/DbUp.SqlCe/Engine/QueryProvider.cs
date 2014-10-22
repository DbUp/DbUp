using System;
using DbUp.Engine.QueryProviders;
using DbUp.Support.SqlServer;

namespace DbUp.SqlCe.Engine
{
    /// <summary>
    /// Return queries for SqlCe
    /// </summary>
    public class QueryProvider : QueryProviderBase
    {
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
                      [VersionId] int identity(1,1) not null constraint PK_{1}_VersionId primary key,
                      [ScriptName] nvarchar(255) not null,
                      [Applied] datetime not null,
                      [Remark] [nvarchar](255) NULL )", SqlObjectParser.QuoteSqlObjectName(TableName), TableName);
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
            return String.Format("SELECT [ScriptName] FROM {0} ORDER BY [ScriptName]", SqlObjectParser.QuoteSqlObjectName(TableName));
        }

        /// <summary>
        /// Sql for inserting new entry in VersionTable
        /// </summary>
        /// <returns>Sql command for inserting new entry in versioning table</returns>
        public override string VersionTableNewEntry()
        {
            return String.Format("INSERT INTO {0} (ScriptName, Applied) VALUES (@scriptName, @applied)", SqlObjectParser.QuoteSqlObjectName(TableName));
        }

        /// <summary>
        /// Sql for checking if version table exists
        /// </summary>
        /// <returns>SQL Command which checks if version table has any entries.</returns>
        public override string VersionTableDoesTableExist()
        {
            return String.Format("SELECT COUNT(*) FROM {0}", SqlObjectParser.QuoteSqlObjectName(TableName));
        }
    }
}
