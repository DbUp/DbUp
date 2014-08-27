using System;
using DbUp.QueryProviders;

namespace DbUp.Oracle
{
    internal sealed class QueryProvider : QueryProviderBase
    {
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
                      CONSTRAINT PK_{0} PRIMARY KEY (ID) ENABLE VALIDATE)", VersionTableName);
        }

        public override string GetVersionTableExecutedScriptsSql()
        {
            return String.Format("SELECT SCRIPT_NAME " +
                                 "FROM {0} " +
                                 "WHERE FAILURE_STATEMENT_INDEX IS NULL AND FAILURE_REMARK IS NULL " +
                                 "ORDER BY SCRIPT_NAME ", VersionTableName);
        }

        public string GetFailedScriptIndex()
        {
            return String.Format("SELECT FAILURE_STATEMENT_INDEX FROM {0} WHERE SCRIPT_NAME = :scriptName", VersionTableName);
        }

        public string GetAppliedScriptHash()
        {
            return String.Format("SELECT SCRIPT_HASHCODE FROM {0} WHERE SCRIPT_NAME = :scriptName", VersionTableName);
        }

        public string DeleteFailedScriptIndex()
        {
            return String.Format(@"DELETE FROM {0} 
                                 WHERE SCRIPT_NAME = :scriptName 
                                 AND FAILURE_STATEMENT_INDEX IS NOT NULL 
                                 AND FAILURE_REMARK IS NOT NULL", VersionTableName);
        }

        public override string VersionTableNewEntry()
        {
            return String.Format("INSERT INTO {0} (SCRIPT_NAME, APPLIED, FAILURE_STATEMENT_INDEX, FAILURE_REMARK, SCRIPT_HASHCODE ) " +
                                 "VALUES (:scriptName, TO_DATE(:applied, 'yyyy-mm-dd hh24:mi:ss'), :failureStatementIndex, :failureRemark, :hash)", VersionTableName);
        }

        public override string VersionTableDoesTableExist()
        {
            return String.Format("SELECT COUNT(*) FROM {0}", VersionTableName);
        }

        public override string CreateSchemeIfNotExists()
        {
            throw new NotImplementedException();
        }
    }
}
