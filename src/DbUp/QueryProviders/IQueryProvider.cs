using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp
{
    /// <summary>
    /// Interface for Queries to specific Database managment system
    /// </summary>
    public interface IQueryProvider
    {
        /// <summary>
        /// Get name table intended for versioning
        /// </summary>
        string VersionTableName
        {
            get;
        }
        /// <summary>
        /// Sql create strign to create versioning table
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
