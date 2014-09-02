using System;
using System.Collections.Generic;
using DbUp.Engine.Transactions;

namespace DbUp.Engine
{
    /// <summary>
    /// Provides scripts to be executed.
    /// </summary>
    public interface IScriptProvider
    {
        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager);

        /// <summary>
        /// Gets scripts to be excluded from all deployments
        /// </summary>
        /// <returns></returns>
        string[] GetExcludedScripts();
    }
}