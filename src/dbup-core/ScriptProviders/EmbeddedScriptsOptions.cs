using System;
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
        /// This will return the two last parts of the resource name: the fileName and extension.
		/// So if the file name does not have an extension, this will not work correctly and it may even throw.
		/// Examples:
		/// resourceName: 'This.Is.The.Full.Resource.Name.AndTheLastTwoIsTheFileName.ThisIsTheFileName.extension' -> fileName: 'ThisIsTheFileName.extension'
		/// resourceName: 'ThisIsTheFileName.extension' -> fileName: 'ThisIsTheFileName.extension'
		/// resourceName: 'This.Is.The.Full.Resource.Name.AndTheLastTwoIsTheFileName.ThisIsTheFileName' -> fileName: 'AndTheLastTwoIsTheFileName.ThisIsTheFileName' (no extension give wrong result)
		/// resourceName: 'ThisIsTheFileName' -> throws ArgumentException
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string FileNameFromResourceName(string resourceName)
        {
            var parts = resourceName.Split('.');
            if (parts.Length < 2)
                throw new ArgumentException(nameof(resourceName), "resourceName can not be split (no dot's)");
            return string.Join(".", parts[parts.Length - 2], parts[parts.Length - 1]);
        }
    }

}
