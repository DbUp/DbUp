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
        private readonly Func<IDbConnection> connectionFactory;
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="connectionFactory">The sql connection factory</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded sql script filter.</param>
        public EmbeddedScriptAndCodeProvider(Func<IDbConnection> connectionFactory, Assembly assembly, Func<string, bool> filter)
        {
            this.connectionFactory = connectionFactory;
            this.assembly = assembly;
            embeddedScriptProvider = new EmbeddedScriptProvider(assembly, filter);
        }

        public IEnumerable<SqlScript> GetScripts()
        {
            var sqlScripts = embeddedScriptProvider
                .GetScripts()
                .Concat(ScriptsFromScriptClasses())
                .OrderBy(x => x.Name)
                .ToList();

            return sqlScripts;
        }

        private IEnumerable<SqlScript> ScriptsFromScriptClasses()
        {
            var script = typeof(IScript);
            return assembly
                .GetTypes()
                .Where(type => script.IsAssignableFrom(type) && type.IsClass)
                .Select(s => (SqlScript)new LazySqlScript(s.FullName + ".cs", () => ((IScript)Activator.CreateInstance(s)).ProvideScript(connectionFactory())))
                .ToList();
        }
    }
}