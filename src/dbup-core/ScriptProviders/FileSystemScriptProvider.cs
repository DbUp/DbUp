using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;

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
        private readonly FileSystemScriptOptions options;
        private readonly SqlScriptOptions sqlScriptOptions;

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        public FileSystemScriptProvider(string directoryPath) : this(directoryPath, new FileSystemScriptOptions(), new SqlScriptOptions())
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="options">Different options for the file system script provider</param>
        public FileSystemScriptProvider(string directoryPath, FileSystemScriptOptions options) : this(directoryPath, options, new SqlScriptOptions())
        {
        }        

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="options">Different options for the file system script provider</param>
        /// <param name="sqlScriptOptions">The sql script options</param>        
        public FileSystemScriptProvider(string directoryPath, FileSystemScriptOptions options, SqlScriptOptions sqlScriptOptions)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            this.directoryPath = directoryPath;
            this.filter = options.Filter;
            this.encoding = options.Encoding;
            this.options = options;
            this.sqlScriptOptions = sqlScriptOptions;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var files = new List<string>();
            foreach (string scriptExtension in options.Extensions)
            {
                files.AddRange(Directory.GetFiles(directoryPath, scriptExtension, ShouldSearchSubDirectories()));
            }
            if (filter != null)
            {
                files = files.Where(filter).ToList();
            }
            return files.Select(x => SqlScript.FromFile(directoryPath, x, encoding, sqlScriptOptions))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private SearchOption ShouldSearchSubDirectories()
        {
            return options.IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        }
    }
}
