using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// An enhanced <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts or IScript code upgrade scripts embedded in an assembly.
    /// </summary>
    public class EmbeddedSqlAndCodeScriptProvider : IScriptProvider
    {
        private readonly EmbeddedSqlScriptProvider embeddedSqlScriptProvider;
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedSqlScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded sql script filter.</param>
        public EmbeddedSqlAndCodeScriptProvider(Assembly assembly, Func<string, bool> filter)
        {
            this.assembly = assembly;
            embeddedSqlScriptProvider = new EmbeddedSqlScriptProvider(assembly, filter);
        }

        private IEnumerable<IScript> ScriptsFromScriptClasses()
        {
            var script = typeof(IScript);
            return assembly
                .GetTypes()
                .Where(type => script.IsAssignableFrom(type) && type.IsClass)
                .Select(s => (IScript) Activator.CreateInstance(s))
                .ToList();
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<IScript> GetScripts()
        {
            var sqlScripts = embeddedSqlScriptProvider
                .GetScripts()
                .Concat(ScriptsFromScriptClasses())
                .OrderBy(x => x.Name)
                .ToList();

            return sqlScripts;
        }
    }
}