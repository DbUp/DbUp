using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DbUp.ScriptProviders
{
    ///<summary>
    /// Alternate <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts via a directory
    ///</summary>
    public class FileSystemScriptProvider : IScriptProvider
    {
        private readonly string _directoryPath;

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        public FileSystemScriptProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts()
        {
            return Directory.GetFiles(_directoryPath, "*.sql").Select(ReadFileAsScript).ToList();
        }

        private static SqlScript ReadFileAsScript(string path)
        {
            string contents = "";
            using (var reader = new StreamReader(path))
            {
                contents = reader.ReadToEnd();
            }
            var fileName = new FileInfo(path).Name;
            return new SqlScript (fileName, contents);

        }
    }
}