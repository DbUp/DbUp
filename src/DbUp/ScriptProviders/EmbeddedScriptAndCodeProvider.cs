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
    public class EmbeddedScriptAndCodeProvider : IScriptProvider
    {
        private readonly EmbeddedScriptProvider embeddedScriptProvider;
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded sql script filter.</param>
        public EmbeddedScriptAndCodeProvider(Assembly assembly, Func<string, bool> filter)
        {
            this.assembly = assembly;
            embeddedScriptProvider = new EmbeddedScriptProvider(assembly, filter);
        }

        private IEnumerable<SqlScript> ScriptsFromScriptClasses(Func<IDbConnection> connectionFactory)
        {
            var script = typeof(IScript);
            return assembly
                .GetTypes()
                .Where(type => script.IsAssignableFrom(type) && type.IsClass)
                .Select(s => (SqlScript)new LazySqlScript(s.FullName + ".cs", () => ((IScript)Activator.CreateInstance(s)).ProvideScript(connectionFactory())))
                .ToList();
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(Func<IDbConnection> connectionFactory)
        {
            var sqlScripts = embeddedScriptProvider
                .GetScripts(connectionFactory)
                .Concat(ScriptsFromScriptClasses(connectionFactory))
                .OrderBy(x => x.Name)
                .ToList();

            return sqlScripts;
        }
    }
}