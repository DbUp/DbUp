using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.ScriptProviders
{
    internal class ScriptInstanceProvider : IScriptProvider
    {
        private readonly IScript[] scripts;
        private readonly Func<IScript, string> namer;
        private readonly ScriptType scriptType;
        private readonly int runOrder;

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="scripts">The IScript instances to include</param>
        public ScriptInstanceProvider(params IScript[] scripts)
            : this(s => s.GetType().FullName + ".cs", scripts)
        {
        }

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="scripts">The IScript instances to include</param>
        /// <param name="namer">A function that returns the name of the script</param>
        public ScriptInstanceProvider(Func<IScript, string> namer, params IScript[] scripts) : this(namer, ScriptType.RunOnce, DbUpDefaults.DefaultRunOrder, scripts)
        {
        }

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="scripts">The IScript instances to include</param>
        /// <param name="namer">A function that returns the name of the script</param>
        /// <param name="scriptType">The type of script.</param>
        public ScriptInstanceProvider(Func<IScript, string> namer, ScriptType scriptType, params IScript[] scripts) : this(namer, scriptType, DbUpDefaults.DefaultRunOrder, scripts)
        {
        }

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="scripts">The IScript instances to include</param>
        /// <param name="namer">A function that returns the name of the script</param>
        /// <param name="scriptType">The script type.</param>
        /// <param name="runOrder">The order the script will be run in</param>
        public ScriptInstanceProvider(Func<IScript, string> namer, ScriptType scriptType, int runOrder, params IScript[] scripts)
        {
            this.scripts = scripts;
            this.namer = namer;
            this.scriptType = scriptType;
            this.runOrder = runOrder;
        }

        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return connectionManager.ExecuteCommandsWithManagedConnection(
                dbCommandFactory => scripts
                    .Select(s => new LazySqlScript(namer(s), scriptType, runOrder, () => s.ProvideScript(dbCommandFactory)))
                    .ToArray()
            );
        }
    }
}