using System;
using System.IO;

namespace DbUp
{
    /// <summary>
    /// Factory class for script namers used in <see cref="ScriptProviders.FileSystemScriptProvider"/>.
    /// </summary>
    // NOTE: DELIBRATELY LEFT IN THE ROOT NAMESPACE
    // Since this class is a helper class that is designed to be available when working with the DbUp Fluent API, 
    // we leave it in the root so that people don't have to manually add using statements to discover it.
    // ReSharper disable CheckNamespace
    public static class FileSystemScriptNamers
    // ReSharper restore CheckNamespace
    {
        /// <summary>
        /// This is the default script namer.
        /// </summary>
        public static Func<string, string> Default()
        {
            return UseFileNames();
        }

        /// <summary>
        /// Use the filename of the script file as name.
        /// </summary>
        /// <example>Will return 'myscript.sql' for a script with path '[script directory]\mysubdir\myscript.sql'</example>
        public static Func<string, string> UseFileNames()
        {
            return (scriptPath) => new FileInfo(scriptPath).Name;
        }

        /// <summary>
        /// Use the relative path of the script file as name.
        /// </summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <example>Will return 'mysubdir\myscript.sql' for a script with path '[scriptroot]\mysubdir\script.sql'</example>
        public static Func<string, string> UseRelativePaths(string directoryPath)
        {
            return (scriptPath) => Path.GetFullPath(scriptPath).Substring(Path.GetFullPath(directoryPath).Length + 1);
        }
    }
}
