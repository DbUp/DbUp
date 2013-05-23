using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// The default <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts embedded in an assembly.
    /// </summary>
    public class EmbeddedSqlScriptProvider : IScriptProvider
    {
        private readonly Assembly assembly;
        private readonly Func<string, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedSqlScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        public EmbeddedSqlScriptProvider(Assembly assembly, Func<string, bool> filter)
        {
            this.assembly = assembly;
            this.filter = filter;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IScript> GetScripts(Func<IDbConnection> connectionFactory)
        {
            return assembly
                .GetManifestResourceNames()
                .Where(filter)
                .OrderBy(x => x)
                .Select(ReadResourceAsScript)
                .Cast<IScript>()
                .ToList();
        }

        private SqlScript ReadResourceAsScript(string scriptName)
        {
            string contents;
            var resourceStream = assembly.GetManifestResourceStream(scriptName);
            using (var resourceStreamReader = new StreamReader(resourceStream))
            {
                contents = resourceStreamReader.ReadToEnd();
            }

            return new SqlScript(scriptName, contents);
        }
    }
}