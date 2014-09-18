using System;
using DbUp.Engine.QueryProviders;

namespace DbUp.Oracle.Engine
{
    /// <summary>
    /// Oracle queries
    /// </summary>
    internal sealed class QueryProvider : QueryProviderBase
    {
        /// <summary>
        /// New Oracle Query provider.
        /// </summary>
        /// <param name="versioningTableName">Name of table which contain versioning. If null use default versioning table name.</param>
        /// <param name="versioningTableScheme"></param>
        public QueryProvider(string versioningTableName = null, string versioningTableScheme = null)
        {
            if (!String.IsNullOrEmpty(versioningTableName))
                this.VersionTableName = versioningTableName;
            if (!String.IsNullOrEmpty(versioningTableScheme))
                this.VersionTableScheme = versioningTableScheme;
        }
        /// <summary>
        ///  Sql create string to create versioning table
        /// </summary>
        /// <returns></returns>
        public override string VersionTableCreationString()
        {
            return String.Format(@"CREATE TABLE {0} (
                      ID integer GENERATED ALWAYS AS IDENTITY(start with 1 increment by 1 nocycle),
                      SCRIPT_NAME VARCHAR2(255) NOT NULL,
                      APPLIED TIMESTAMP NOT NULL,
                      REMARK VARCHAR2(255) NULL,
                      FAILURE_STATEMENT_INDEX integer NULL,
                      FAILURE_REMARK VARCHAR2(255) NULL,
                      SCRIPT_HASHCODE integer NULL,
                      CONSTRAINT PK_{0} PRIMARY KEY (ID) ENABLE VALIDATE)", TableName);
        }

        /// <summary>
        /// Sql string getting which scipt names are in versioning table
        /// </summary>
        /// <returns></returns>
        public override string GetVersionTableExecutedScriptsSql()
        {
            return String.Format("SELECT SCRIPT_NAME " +
                                 "FROM {0} " +
                                 "WHERE FAILURE_STATEMENT_INDEX IS NULL AND FAILURE_REMARK IS NULL " +
                                 "ORDER BY SCRIPT_NAME ", TableName);
        }
        /// <summary>
        /// Sql string getting failure statement index for script in versioning table
        /// </summary>
        /// <remarks>
        /// Params: 
        ///   - :scriptName
        /// </remarks>
        /// <returns></returns>
        public string GetFailedScriptIndex()
        {
            return String.Format("SELECT FAILURE_STATEMENT_INDEX FROM {0} WHERE SCRIPT_NAME = :scriptName", TableName);
        }
        /// <summary>
        /// Sql string getting executed script hash code
        /// </summary>
        /// <remarks>
        /// Params: 
        ///   - :scriptName
        /// </remarks>
        /// <returns></returns>
        public string GetAppliedScriptHash()
        {
            return String.Format("SELECT SCRIPT_HASHCODE FROM {0} WHERE SCRIPT_NAME = :scriptName", TableName);
        }
        /// <summary>
        ///  Sql string for deleting entry for passed script_name in versioning table.
        /// </summary>
        /// <remarks>
        /// Params: 
        ///   - :scriptName
        /// </remarks>
        /// <returns></returns>
        public string DeleteFailedScriptIndex()
        {
            return String.Format(@"DELETE FROM {0} 
                                 WHERE SCRIPT_NAME = :scriptName 
                                 AND FAILURE_STATEMENT_INDEX IS NOT NULL 
                                 AND FAILURE_REMARK IS NOT NULL", TableName);
        }
        /// <summary>
        /// Sql string, inserting new entry in VersionTable. 
        /// </summary>
        /// <remarks>
        /// Params: 
        ///   - :scriptName
        ///   - :applied
        ///   - :failureStatementIndex
        ///   - :failureRemark
        ///   - :hash
        /// </remarks>
        /// <returns></returns>
        public override string VersionTableNewEntry()
        {
            return String.Format("INSERT INTO {0} (SCRIPT_NAME, APPLIED, FAILURE_STATEMENT_INDEX, FAILURE_REMARK, SCRIPT_HASHCODE ) " +
                                 "VALUES (:scriptName, TO_DATE(:applied, 'yyyy-mm-dd hh24:mi:ss'), :failureStatementIndex, :failureRemark, :hash)", TableName);
        }
        /// <summary>
        /// Sql string, checking if version table exists
        /// </summary>
        /// <returns></returns>
        public override string VersionTableDoesTableExist()
        {
            return String.Format("SELECT COUNT(*) FROM {0}", TableName);
        }
        /// <summary>
        /// Not implemented!!!
        /// </summary>
        /// <returns></returns>
        public override string CreateSchemeIfNotExists()
        {
            throw new NotImplementedException();
        }
    }
}
