using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.QueryProviders
{
    /// <summary>
    /// Return queries for SqlCe
    /// </summary>
    public sealed class SqlCeQueryProvider : IQueryProvider
    {
        private const string JournalTableName = "SchemaVersions";

        /// <summary>
        /// Get name table intended for versioning
        /// </summary>
        public string VersionTableName
        {
            get { return JournalTableName; }
        }

        /// <summary>
        /// Sql create strign to create versioning table
        /// </summary>
        /// <returns>Sql command for creating of version table</returns>
        public string VersionTableCreationString()
        {
            return String.Format(@"CREATE TABLE [{0}] (
                      [VersionId] int identity(1,1) not null constraint PK_{0}_VersionId primary key,
                      [ScriptName] nvarchar(255) not null,
                      [Applied] datetime not null,
                      [Remark] [nvarchar](255) NULL )", JournalTableName) ;
        }

        /// <summary>
        /// Sql string for checking if scheme exists and if not create new scheme.
        /// </summary>
        /// <returns></returns>
        public string CreateSchemeIfNotExists()
        {
            return @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA [{0}]')";
        }
        /// <summary>
        /// Sql for getting which scipt names are in versioning table
        /// </summary>
        /// <returns>Sql command for selecting scirpt names from VersionTableName</returns>
        public string GetVersionTableExecutedScriptsSql()
        {
            return String.Format("SELECT [ScriptName] FROM {0} ORDER BY [ScriptName]", JournalTableName);
        }

        /// <summary>
        /// Sql for inserting new entry in VersionTable
        /// </summary>
        /// <returns>Sql command for inserting new entry in versioning table</returns>
        public string VersionTableNewEntry()
        {
            return String.Format("INSERT INTO {0} (ScriptName, Applied) VALUES (@scriptName, @applied)", JournalTableName);
        }

        /// <summary>
        /// Sql for checking if version table exists
        /// </summary>
        /// <returns>SQL Command which checks if version table has any entries.</returns>
        public string VersionTableDoesTableExist()
        {
             return String.Format("SELECT COUNT(*) FROM {0}", JournalTableName);
        }
    }
}
