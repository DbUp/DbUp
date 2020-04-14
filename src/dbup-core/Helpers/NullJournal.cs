using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Engine;

namespace DbUp.Helpers
{
    /// <summary>
    /// Enables multiple executions of idempotent scripts.
    /// </summary>
    public class NullJournal : IJournal
    {
        /// <summary>
        /// Returns an empty array of length 0.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExecutedSqlScript> GetExecutedScripts() => Enumerable.Empty<ExecutedSqlScript>();

        /// <summary>
        /// Does not store the script, simply returns.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="dbCommandFactory"></param>
        public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
        { }

        /// <summary>
        /// Does not ensure table exists, simply returns.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="dbCommandFactory"></param>
        public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        { }
    }
}
