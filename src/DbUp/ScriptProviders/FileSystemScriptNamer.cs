using System;
using System.IO;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// Packaged script namers for <see cref="FileSystemScriptOptions.ScriptNamer"/>
    /// </summary>
    public static class FileSystemScriptNamer
    {
        /// <summary>
        /// Script namer for using only the filename of the script.
        /// </summary>
        /// <example>script at path '[script directory]\mysubdir\myscript.sql' will be named as 'myscript.sql'</example>
        public static Func<string, string> Filename()
        {
            return (scriptPath) => new FileInfo(scriptPath).Name;
        }

        /// <summary>
        /// Script namer to use when the relative path to the script should be part of the script name.
        /// </summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <example>script at path '[scriptroot]\mysubdir\script.sql' will be named as 'mysubdir\myscript.sql'</example>
        public static Func<string, string> RelativePath(string directoryPath)
        {
            return (scriptPath) => Path.GetFullPath(scriptPath).Substring(Path.GetFullPath(directoryPath).Length + 1);
        }
    }
}