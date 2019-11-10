using System;
using System.Linq;

namespace DbUp
{
    /// <summary>
    /// A factory class for filter methods.
    /// </summary>
    // NOTE: DELIBRATELY LEFT IN THE ROOT NAMESPACE
    // Since this class is a helper class that is designed to be available when working with the DbUp Fluent API, 
    // we leave it in the root so that people don't have to manually add using statements to discover it.
    // ReSharper disable CheckNamespace
    public static class Filters
    // ReSharper restore CheckNamespace
    {
        /// <summary>
        /// This filter will exclude scripts that are listed in a file.
        /// </summary>
        ///<remarks>
        /// The file should contain a single script name per line.
        ///</remarks>
        /// <param name="fileName">The file that contains the script names to be excluded, one per line.</param>
        /// <returns></returns>
        public static Func<string, bool> ExcludeScriptNamesInFile(string fileName)
        {
            // read script names from text file into a list
            var scriptNames = System.IO.File.ReadAllLines(fileName).ToArray();
            return ExcludeScripts(scriptNames);
        }

        /// <summary>
        /// This filter will only include scripts that are listed in a file. All other scripts will be excluded.
        /// </summary>
        ///<remarks>
        ///The file should contain a single script name per line.
        ///</remarks>
        /// <param name="fileName">The file that contains the script names to be included, one per line.</param>
        /// <returns></returns>
        public static Func<string, bool> OnlyIncludeScriptNamesInFile(string fileName)
        {
            // read script names from text file into a list
            var scriptNames = System.IO.File.ReadAllLines(fileName).ToArray();
            return OnlyIncludeScripts(scriptNames);
        }

        /// <summary>
        /// This filter will exclude the specified scripts.
        /// </summary>    
        /// <param name="scriptNames">The names of the scripts to be excluded.</param>
        /// <returns></returns>
        public static Func<string, bool> ExcludeScripts(params string[] scriptNames) => s => !scriptNames.Contains(s);

        /// <summary>
        /// This filter will include only the specified scripts, and exclude any others.
        /// </summary>    
        /// <param name="scriptNames">The names of the scripts to be included.</param>
        /// <returns></returns>
        public static Func<string, bool> OnlyIncludeScripts(params string[] scriptNames) => s => scriptNames.Contains(s);
    }
}


