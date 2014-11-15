using DbUp.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.Postgresql
{
    /// <summary>
    /// Return queries for Postgre
    /// </summary>
    public class PostgresqlStatements : SqlStatementsContainer
    {
        /// <summary>
        /// Full name for versioning table ("@sheme"."@tableName")
        /// </summary>
        public string SchemaTableName = "";

        /// <summary>
        /// New queries container for Postgre
        /// </summary>
        /// <param name="versioningTableName">Name of table which contains versions</param>
        /// <param name="schema">Schema name of table which handle versioning. If null, queries will not include shema in queries. </param>
        public PostgresqlStatements(string versioningTableName = null, string schema = null)
        {
            if (!String.IsNullOrEmpty(versioningTableName))
                this.VersionTableName = versioningTableName;

            SchemaTableName = PostgreObjectParser.QuoteSqlObjectName(this.VersionTableName);
            if (string.IsNullOrEmpty(schema))
                SchemaTableName = PostgreObjectParser.QuoteSqlObjectName(this.VersionTableName);
            else
            {
                SchemaTableName = PostgreObjectParser.QuoteSqlObjectName(schema) + "." + PostgreObjectParser.QuoteSqlObjectName(this.VersionTableName);
                this.Scheme = schema;
            }
        }

        /// <summary>
        /// Sql create string to create versioning table
        /// </summary>
        /// <returns>Sql command for creating of version table</returns>
        public override string VersionTableCreationString()
        {
            return String.Format(
                            @"CREATE TABLE {0}
                              (
                                schemaversionsid serial NOT NULL,
                                scriptname character varying(255) NOT NULL,
                                applied timestamp without time zone NOT NULL,
                                CONSTRAINT pk_schemaversions_id PRIMARY KEY (schemaversionsid)
                              )", SchemaTableName);

            //SQLiteObjectParser.QuoteSqlObjectName(TableName), TableName);
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
