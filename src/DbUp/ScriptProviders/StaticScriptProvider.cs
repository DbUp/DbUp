using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// Allows you to easily programatically supply scripts from code.
    /// </summary>
    public sealed class StaticScriptProvider : IScriptProvider
    {
        private readonly IEnumerable<SqlScript> scripts;
        private readonly IEnumerable<string> excludedScripts;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticScriptProvider"/> class.
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        /// <param name="excludedScripts">Scripts to be excluded</param>
        public StaticScriptProvider(IEnumerable<SqlScript> scripts, IEnumerable<string> excludedScripts)
        {
            this.scripts = scripts;
            this.excludedScripts = excludedScripts;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return scripts;
        }

        /// <summary>
        /// Gets all scripts that should be excluded.
        /// </summary>
        /// <returns></returns>
        public string[] GetExcludedScripts()
        {
            return null != excludedScripts ? excludedScripts.ToArray() : new string[0];
        }
    }
}