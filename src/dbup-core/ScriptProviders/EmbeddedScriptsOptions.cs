using System;
using System.Linq;
using System.Text;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    public class EmbeddedScriptsOptions
    {
        /// <summary>
        /// The filter.
        /// </summary>
        public Func<string, bool> Filter { get; set; }

        /// <summary>
        /// A function that returns the name of the script.
        /// </summary>
        public Func<string, string> ScriptNameFromResourceName { get; set; }

        /// <summary>
        /// The encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The sql script options.
        /// </summary>
        public SqlScriptOptions SqlScriptOptions { get; set; }

        /// <summary>
        /// Return the two last parts of the resource name: fileName and extension.
        /// File name must have an extension for this to work correctly.
        /// Examples:
        /// 'Assembly.Namespace.FileName.ext' -> 'FileName.ext'
        /// 'FileName.ext' -> 'FileName.ext'
        /// 'FileName' -> 'FileName'
        /// 'Assembly.Namespace.FileName' -> 'Namespace.FileName' (wrong)
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string FileNameFromResourceName(string resourceName)
        {
            var parts = resourceName.Split('.');
            return parts.Length >= 2 ? string.Join(".", parts[parts.Length - 2], parts[parts.Length - 1]) : parts.Single();
        }
    }

}
