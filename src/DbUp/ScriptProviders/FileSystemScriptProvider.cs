using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.ScriptProviders
{
    ///<summary>
    /// Alternate <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts via a directory
    ///</summary>
    public class FileSystemScriptProvider : IScriptProvider
    {
        private readonly string directoryOrFilePath;

        ///<summary>
        ///</summary>
        ///<param name="directoryOrFilePath">Path to SQL upgrade scripts</param>
        public FileSystemScriptProvider(string directoryOrFilePath)
        {
            this.directoryOrFilePath = directoryOrFilePath;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            if (Directory.Exists(directoryOrFilePath))
            {
                return Directory.GetFiles(directoryOrFilePath, "*.sql").Select(SqlScript.FromFile).ToList();
            }

            if (File.Exists(directoryOrFilePath))
            {
                return new[] {SqlScript.FromFile(directoryOrFilePath)};
            }

            return new List<SqlScript>();
        }

        /// <summary>
        /// Returns excluded scripts array from DbUpExcludedFiles.txt
        /// </summary>
        /// <returns></returns>
        public string[] GetExcludedScripts()
        {
            string filePath = Path.Combine(directoryOrFilePath, "DbUpExcludedFiles.txt");
            return File.Exists(filePath) ? File.ReadAllLines(filePath) : new string[0];
        }
    }
}