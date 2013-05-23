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
    public class EmbeddedSqlScriptAndCodeProvider : IScriptProvider
    {
        private readonly EmbeddedSqlScriptProvider embeddedSqlScriptProvider;
        private readonly Assembly assembly;
        private readonly Func<string, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedSqlScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded sql script filter.</param>
        public EmbeddedSqlScriptAndCodeProvider(Assembly assembly, Func<string, bool> filter)
        {
            this.assembly = assembly;
            this.filter = filter;
            embeddedSqlScriptProvider = new EmbeddedSqlScriptProvider(assembly, filter);
        }

        private IEnumerable<IScript> ScriptsFromScriptClasses(Func<IDbConnection> connectionFactory)
        {
            var script = typeof(IScript);
            return assembly
                .GetTypes()
                .Where(type => script.IsAssignableFrom(type) && type.IsClass)
                .Where(t => filter(t.FullName))
                .Select(s => (IScript)Activator.CreateInstance(s))
                .ToList();
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<IScript> GetScripts(Func<IDbConnection> connectionFactory)
        {
            var sqlScripts = embeddedSqlScriptProvider
                .GetScripts(connectionFactory)
                .Concat(ScriptsFromScriptClasses(connectionFactory))
                .OrderBy(x => x.Name)
                .ToList();

            return sqlScripts;
        }
    }
}