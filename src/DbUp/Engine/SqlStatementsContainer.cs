using System;

namespace DbUp.Engine
{
    /// <summary>
    /// Class for supporting query string which are same for all providers
    /// </summary>
    public abstract class SqlStatementsContainer
    {
        /// <summary>
        /// Name of Journal table in database.
        /// </summary>
        protected string VersionTableName = "SchemaVersions";
        /// <summary>
        /// Scheme of Journal table in database.
        /// </summary>
        protected string VersionTableScheme = null;

        /// <summary>
        /// Name of versioning table
        /// </summary>
        public string TableName 
        {
            get { return VersionTableName; }
        }
        /// <summary>
        /// Scheme of versioning table
        /// </summary>
        public string Scheme
        {
            get { return VersionTableScheme; }
        }
        /// <summary>
        /// Abstract method for Sql create string to create versioning table
        /// </summary>
        /// <returns></returns>
        public abstract string VersionTableCreationString();
        /// <summary>
        /// Abstract method for Sql string getting which scipt names are in versioning table
        /// </summary>
        /// <returns></returns>
        public abstract string GetVersionTableExecutedScriptsSql();
        /// <summary>
        /// Abstract method for Sql string, inserting new entry in VersionTable
        /// </summary>
        /// <returns></returns>
        public abstract string VersionTableNewEntry();
        /// <summary>
        /// Abstract method for Sql string, checking if version table exists
        /// </summary>
        /// <returns></returns>
        public abstract string VersionTableDoesTableExist();
        /// <summary>
        /// Abstract method for Sql string intended for checking if scheme exists and if not create new scheme.
        /// </summary>
        /// <returns></returns>
        public abstract string CreateSchemeIfNotExists();
    }
}
