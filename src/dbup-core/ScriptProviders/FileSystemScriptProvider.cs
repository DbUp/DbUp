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
        private readonly string directoryPath;
        private readonly Func<string, bool> filter;
        private readonly Encoding encoding;

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="filter">The filter.</param>
        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter = null)
            : this(directoryPath, filter, DbUpDefaults.DefaultEncoding)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="encoding">The encoding.</param>
        public FileSystemScriptProvider(string directoryPath, Encoding encoding)
        {
            this.directoryPath = directoryPath;
            filter = null;
            this.encoding = encoding;
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="filter">The filter.</param>
        ///<param name="encoding">The encoding.</param>
        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter, Encoding encoding)
        {
            this.directoryPath = directoryPath;
            this.filter = filter;
            this.encoding = encoding;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var files = Directory.GetFiles(directoryPath, "*.sql").AsEnumerable();
            if (filter != null)
            {
                files = files.Where(filter);
            }
            return files.Select(x => SqlScript.FromFile(x, encoding)).ToList();
        }


    }
}