using DbUp.Engine.Transactions;
using System.Collections.Generic;

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
    }
}