using System;
using DbUp.Engine;

namespace DbUp.Helpers
{
    /// <summary>
    /// Enables multiple executions of a set of scripts.
    /// </summary>
    public class NullJournal : IJournal
    {
        /// <summary>
        /// Returns an empy array of length 0 to ensure all 
        /// scripts are run
        /// </summary>
        /// <returns></returns>
        public string[] GetExecutedScripts()
        {
            return new string[0];
        }

        /// <summary>
        /// Does not store the script, simply returns
        /// </summary>
        /// <param name="script"></param>
        public void StoreExecutedScript(SqlScript script)
        { }
    }
}
