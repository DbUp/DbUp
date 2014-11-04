using System;
using DbUp.Engine;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// Return queries for Microsoft Sql Server
    /// </summary>
    public class SqlServerStatementsContainer : SqlStatementsContainer
    {
        /// <summary>
        /// Full name for versioning table ([@sheme].[@tableName])
        /// </summary>
        public string SchemaTableName = "dbo";
        /// <summary>
        /// Create new SqlServer Query provider.
        /// </summary>
        /// <param name="tableName">Plain name of table which handle versioning. If passed name is null or "", take default name SchemaVersions</param>
        /// <param name="schema">Schema name of table which handle versioning. If null, queries will not include shema in queries. </param>
        public SqlServerStatementsContainer(string tableName = null, string schema = null)
        {
            if (!String.IsNullOrEmpty(tableName))
                this.VersionTableName = tableName;

            SchemaTableName = SqlObjectParser.QuoteSqlObjectName(this.VersionTableName);
            if (string.IsNullOrEmpty(schema))
                SchemaTableName = SqlObjectParser.QuoteSqlObjectName(this.VersionTableName);
            else
            {
                SchemaTableName = SqlObjectParser.QuoteSqlObjectName(schema) + "." + SqlObjectParser.QuoteSqlObjectName(this.VersionTableName);
                this.VersionTableScheme = schema;
            }
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
                      [Remark] [nvarchar](255) NULL ) ", SchemaTableName, VersionTableName);
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
            return String.Format("SELECT ScriptName FROM {0} ORDER BY ScriptName", SchemaTableName);
        }

        /// <summary>
        /// Sql for inserting new entry in VersionTable
        /// </summary>
        /// <returns>Sql command for inserting new entry in versioning table</returns>
        public override string VersionTableNewEntry()
        {
            return String.Format("INSERT INTO {0} (ScriptName, Applied) VALUES (@scriptName, @applied)", SchemaTableName);
        }

        /// <summary>
        /// Sql for checking if version table exists
        /// </summary>
        /// <returns>SQL Command which checks if version table has any entries.</returns>
        public override string VersionTableDoesTableExist()
        {
            return String.Format("SELECT COUNT(*) FROM {0}", SchemaTableName);
        }
    }
}
