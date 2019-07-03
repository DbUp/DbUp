using System;
using System.Data;
using DbUp.Engine;

namespace DbUp.Helpers
{
    /// <summary>
    /// Enables multiple executions of idempotent scripts.
    /// </summary>
    public class NullJournal : IJournal
    {
        /// <summary>
        /// Returns an empty array of length 0
        /// </summary>
        /// <returns></returns>
        public ExecutedSqlScript[] GetExecutedScripts()
        {
            return new ExecutedSqlScript[0];
        }

        /// <summary>
        /// Does not store the script, simply returns
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="processedContents">The processed contents.</param>
        /// <param name="dbCommandFactory">The database command factory.</param>
        public void StoreExecutedScript(SqlScript script,string processedContents, Func<IDbCommand> dbCommandFactory)
        { }

        public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        { }
    }
}
