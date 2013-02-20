using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    ///<summary>
    /// Alternate <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts via a directory
    ///</summary>
    public class FileSystemScriptProvider : IScriptProvider
    {
        private readonly string directoryPath;

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        public FileSystemScriptProvider(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<IScript> GetScripts()
        {
            return Directory.GetFiles(directoryPath, "*.sql").Select(SqlScript.FromFile).Cast<IScript>().ToList();
        }


    }



}