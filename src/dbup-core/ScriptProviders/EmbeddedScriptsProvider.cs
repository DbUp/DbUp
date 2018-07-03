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
        private readonly Assembly[] assemblies;
        private readonly Encoding encoding;
        private readonly Func<string, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptsProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        public EmbeddedScriptsProvider(Assembly[] assemblies, Func<string, bool> filter, Encoding encoding)
        {
            this.assemblies = assemblies;
            this.filter = filter;
            this.encoding = encoding;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var sqlScripts = assemblies
                .Select(assembly => new
                {
                    Assembly = assembly,
                    ResourceNames = assembly.GetManifestResourceNames().Where(filter).ToArray()
                })
                .SelectMany(x => x.ResourceNames.Select(resourceName => SqlScript.FromStream(resourceName, x.Assembly.GetManifestResourceStream(resourceName), encoding)))
                .OrderBy(sqlScript => sqlScript.Name)
                .ToList();

            return sqlScripts;
        }
    }
}