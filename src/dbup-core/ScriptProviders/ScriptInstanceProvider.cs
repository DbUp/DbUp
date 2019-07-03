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
        private readonly IHasher hasher;
        private readonly IScript[] scripts;
        private readonly Func<IScript, string> namer;
        private readonly SqlScriptOptions sqlScriptOptions;

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="scripts">The IScript instances to include</param>
        public ScriptInstanceProvider(IHasher hasher, params IScript[] scripts)
            : this(s => s.GetType().FullName + ".cs", hasher, scripts)
        {
        }

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="namer">A function that returns the name of the script</param>
        /// <param name="scripts">The IScript instances to include</param>
        /// <param name="hasher">The hasher.</param>
        public ScriptInstanceProvider(Func<IScript, string> namer, IHasher hasher, params IScript[] scripts) : this(namer, new SqlScriptOptions(), hasher, scripts)
        {
            this.hasher = hasher;
        }

        /// <summary>
        /// Provider used to directly include an IScript instance during migrations
        /// </summary>
        /// <param name="namer">A function that returns the name of the script</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>
        /// <param name="hasher">The hasher.</param>
        /// <param name="scripts">The IScript instances to include</param>
        public ScriptInstanceProvider(Func<IScript, string> namer, SqlScriptOptions sqlScriptOptions, IHasher hasher, params IScript[] scripts)
        {
            this.scripts = scripts;
            this.namer = namer;
            this.sqlScriptOptions = sqlScriptOptions;
            this.hasher = hasher;
        }

        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return connectionManager.ExecuteCommandsWithManagedConnection(
                dbCommandFactory => scripts
                    .Select(s => new LazySqlScript(namer(s), sqlScriptOptions, () => s.ProvideScript(dbCommandFactory), s1 => hasher.GetHash(s1)))
                    .ToArray()
            );
        }
    }
}