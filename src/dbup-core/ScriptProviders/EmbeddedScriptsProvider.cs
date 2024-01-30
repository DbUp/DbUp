namespace DbUp.ScriptProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Engine;
    using Engine.Transactions;

    /// <summary>
    /// An <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts embedded in assemblies.
    /// </summary>
    public class EmbeddedScriptsProvider : IScriptProvider
    {
        readonly Assembly[] assemblies;
        readonly Encoding encoding;
        readonly Func<string, bool> filter;
        readonly Func<string, string> scriptNameFromResourceName;
        readonly SqlScriptOptions sqlScriptOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptsProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        public EmbeddedScriptsProvider(Assembly[] assemblies, Func<string, bool> filter, Encoding encoding) : this(assemblies, filter, encoding, new SqlScriptOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptsProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>
        public EmbeddedScriptsProvider(Assembly[] assemblies, Func<string, bool> filter, Encoding encoding, SqlScriptOptions sqlScriptOptions)
            : this(assemblies, filter, resourceName => resourceName, encoding, sqlScriptOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptsProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="scriptNameFromResourceName">A function that returns the name of the script.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>
        public EmbeddedScriptsProvider(Assembly[] assemblies, Func<string, bool> filter, Func<string, string> scriptNameFromResourceName, Encoding encoding, SqlScriptOptions sqlScriptOptions)
        {
            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
            this.scriptNameFromResourceName = scriptNameFromResourceName ?? throw new ArgumentNullException(nameof(scriptNameFromResourceName));
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.sqlScriptOptions = sqlScriptOptions;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return assemblies
                .Select(assembly => new
                {
                    Assembly = assembly,
                    ResourceNames = assembly.GetManifestResourceNames().Where(filter).ToArray()
                })
                .SelectMany(x => x.ResourceNames.Select(resourceName => SqlScript.FromStream(scriptNameFromResourceName(resourceName), x.Assembly.GetManifestResourceStream(resourceName), encoding, sqlScriptOptions)))
                .OrderBy(sqlScript => sqlScript.Name)
                .ToList();
        }
    }
}
