using DbUp.Engine;
using System.Collections.Generic;
using System;

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
        public string[] GetExecutedScripts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Does not store the script, simply returns
        /// </summary>
        /// <param name="script"></param>
        public void StoreExecutedScript(SqlScript script)
        { }

        
    }
}
