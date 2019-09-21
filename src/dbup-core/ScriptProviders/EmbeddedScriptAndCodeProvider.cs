using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// An enhanced <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts or IScript code upgrade scripts embedded in an assembly.
    /// </summary>
    public class EmbeddedScriptAndCodeProvider : IScriptProvider
    {
        readonly EmbeddedScriptProvider embeddedScriptProvider;
        readonly Assembly assembly;
        readonly Func<string, bool> filter;
        readonly SqlScriptOptions sqlScriptOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded script and code file filter.</param>
        public EmbeddedScriptAndCodeProvider(Assembly assembly, Func<string, bool> filter)
            : this(assembly, filter, filter, new SqlScriptOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded script filter.</param>
        /// <param name="codeScriptFilter">The embedded script filter. If null, filter is used.</param>
        public EmbeddedScriptAndCodeProvider(Assembly assembly, Func<string, bool> filter, Func<string, bool> codeScriptFilter) : this(assembly, filter, codeScriptFilter, new SqlScriptOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded script and code file filter.</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>  
        public EmbeddedScriptAndCodeProvider(Assembly assembly, Func<string, bool> filter, SqlScriptOptions sqlScriptOptions)
            : this(assembly, filter, filter, sqlScriptOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The embedded script filter.</param>
        /// <param name="codeScriptFilter">The embedded script filter. If null, filter is used.</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>        
        public EmbeddedScriptAndCodeProvider(Assembly assembly, Func<string, bool> filter, Func<string, bool> codeScriptFilter, SqlScriptOptions sqlScriptOptions)
        {
            this.assembly = assembly;
            this.filter = codeScriptFilter ?? filter;
            this.sqlScriptOptions = sqlScriptOptions;
            embeddedScriptProvider = new EmbeddedScriptProvider(assembly, filter);
        }

        IEnumerable<SqlScript> ScriptsFromScriptClasses(IConnectionManager connectionManager)
        {
            var script = typeof(IScript);
            return connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory => assembly
                .GetTypes()
                .Where(type =>
                {
                    return script.IsAssignableFrom(type) &&
#if USE_TYPE_INFO
                        type.GetTypeInfo().IsClass &&
                       !type.GetTypeInfo().IsAbstract;
#else
                        type.IsClass &&
                       !type.IsAbstract;
#endif
                })
                .Select(s => (SqlScript)new LazySqlScript(s.FullName + ".cs", sqlScriptOptions, () => ((IScript)Activator.CreateInstance(s)).ProvideScript(dbCommandFactory)))
                .ToList());
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var sqlScripts = embeddedScriptProvider
                .GetScripts(connectionManager)
                .Concat(ScriptsFromScriptClasses(connectionManager))
                .OrderBy(x => x.Name)
                .Where(x => filter(x.Name))
                .ToList();

            return sqlScripts;
        }
    }
}