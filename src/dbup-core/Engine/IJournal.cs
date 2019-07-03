using System;
using System.Data;

namespace DbUp.Engine
{
    /// <summary>
    /// This interface is provided to allow different projects to store version information differently.
    /// </summary>
    public interface IJournal
    {
        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns></returns>
        ExecutedSqlScript[] GetExecutedScripts();

        /// <summary>
        /// Records an upgrade script for a database.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="processedContents">The processed contents.</param>
        /// <param name="dbCommandFactory">The database command factory.</param>
        void StoreExecutedScript(SqlScript script,string processedContents, Func<IDbCommand> dbCommandFactory);

        /// <summary>
        /// Creates the journal if it does not exist, and if it does exist makes sure it is in the latest format
        /// This is called just before a script is executed
        /// </summary>
        void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory);
    }
}