using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// Allows you to easily programatically supply scripts from code.
    /// </summary>
    public sealed class StaticScriptProvider : IScriptProvider
    {
        private readonly IEnumerable<IScript> scripts;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticScriptProvider"/> class.
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        public StaticScriptProvider(IEnumerable<IScript> scripts)
        {
            this.scripts = scripts;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<IScript> GetScripts()
        {
            return scripts;
        }
    }
}