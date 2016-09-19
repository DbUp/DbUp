using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using System.Linq;

namespace CommandLineApplication.DbUp
{
    public static class UpgradeEngineExtensions
    {
        /// <summary>
        ///     Adds all scripts found as embedded resources in the given assembly, with a custom filter (you'll need to exclude
        ///     non- .SQL files yourself).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="scriptNamespace"></param>
        /// <param name="stableScripts"></param>
        /// <returns>The same builder</returns>
        public static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, string scriptNamespace, bool stableScripts)
        {
            return builder.WithScripts(new EmbeddedStableScriptsProvider(new[] {assembly}, scriptNamespace, stableScripts));
        }
    }

    public class EmbeddedStableScriptsProvider : IScriptProvider
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DbUp.ScriptProviders.EmbeddedScriptsProvider" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <param name="scriptNamespace">Namespace of scripts</param>
        /// <param name="stableScripts">Only execute stable scripts</param>
        public EmbeddedStableScriptsProvider(Assembly[] assemblies, string scriptNamespace, bool stableScripts)
        {
            _assemblies = assemblies;
            _scriptNamespace = scriptNamespace;
            _stableScripts = stableScripts;
        }

        /// <summary>Gets all scripts that should be executed.</summary>
        /// <returns></returns>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var sqlScripts = new List<SqlScript>();
            foreach (var assembly in _assemblies)
            {
                var resourceNames = assembly.GetManifestResourceNames().Where(Filter).ToArray();
                foreach (var name in resourceNames)
                {
                    sqlScripts.Add(SqlScript.FromStream(name, assembly.GetManifestResourceStream(name), Encoding.Default));
                }
            }

            // ensure stable scripts are executed first
            var stable = sqlScripts.Where(x => x.Name.Contains(".Stable")).ToArray();
            var other = sqlScripts.Except(stable).ToArray();

            var orderedScripts = new List<SqlScript>();
            orderedScripts.AddRange(stable);
            orderedScripts.AddRange(other);

            return orderedScripts;
        }

        private bool Filter(string fileName)
        {
            if (!fileName.EndsWith(".sql"))
                return false;

            if (!fileName.StartsWith(_scriptNamespace))
                return false;

            if (_stableScripts)
                return fileName.Contains(".Stable");

            return true;
        }

        private readonly Assembly[] _assemblies;
        private readonly string _scriptNamespace;
        private readonly bool _stableScripts;
    }
}
