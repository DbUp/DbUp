using System;
using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.ScriptProviders
{
    class ScriptInstanceProvider : IScriptProvider
    {
        readonly IScript[] scripts;
        readonly Func<IScript, string> namer;
        readonly SqlScriptOptions sqlScriptOptions;

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
        public ScriptInstanceProvider(Func<IScript, string> namer, params IScript[] scripts) : this(namer, new SqlScriptOptions(), scripts)
        {
        }

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="scripts">The IScript instances to include</param>
        /// <param name="namer">A function that returns the name of the script</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>        
        public ScriptInstanceProvider(Func<IScript, string> namer, SqlScriptOptions sqlScriptOptions, params IScript[] scripts)
        {
            this.scripts = scripts;
            this.namer = namer ?? throw new ArgumentNullException(nameof(namer));
            this.sqlScriptOptions = sqlScriptOptions;
        }

        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return connectionManager.ExecuteCommandsWithManagedConnection(
                dbCommandFactory => scripts
                    .Select(s => new LazySqlScript(namer(s), sqlScriptOptions, () => s.ProvideScript(dbCommandFactory)))
                    .ToArray()
            );
        }
    }
}
