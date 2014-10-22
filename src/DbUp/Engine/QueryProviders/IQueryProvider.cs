using System;

namespace DbUp
{
    /// <summary>
    /// Interface for Sql queries for specific Database managment system
    /// </summary>
    public interface IQueryProvider
    {
        /// <summary>
        /// Name of versioning table
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// Scheme of versioning table
        /// </summary>
        string Scheme { get; }
        /// <summary>
        /// Sql create string to create versioning table
        /// </summary>
        /// <returns>Sql command for creating of version table</returns>
        string VersionTableCreationString();

        /// <summary>
        /// Sql for getting which scipt names are in versioning table
        /// </summary>
        /// <returns>Sql command for selecting scirpt names from VersionTableName</returns>
        string GetVersionTableExecutedScriptsSql();

        /// <summary>
        /// Sql for inserting new entry in VersionTable
        /// </summary>
        /// <returns>Sql command for inserting new entry in versioning table</returns>
        string VersionTableNewEntry();

        /// <summary>
        /// Sql for checking if version table exists
        /// </summary>
        /// <returns>SQL Command which checks if version table has any entries.</returns>
        string VersionTableDoesTableExist();

        /// <summary>
        /// Sql string for checking if scheme exists and if not create new scheme.
        /// </summary>
        /// <returns></returns>
        string CreateSchemeIfNotExists();
    }
}
