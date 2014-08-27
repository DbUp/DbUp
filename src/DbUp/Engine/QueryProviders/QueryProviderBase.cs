﻿using System;

namespace DbUp.QueryProviders
{
    /// <summary>
    /// Class for supporting query string which are same for all providers
    /// </summary>
    public abstract class QueryProviderBase : IQueryProvider
    {
        /// <summary>
        /// Name of Journal table in database.
        /// </summary>
        public static string VersionTableName = "DB_VERSION";
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
