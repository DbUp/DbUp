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
        private readonly ScriptType scriptType;
        private readonly int runOrder;

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        public FileSystemScriptProvider(string directoryPath) : this(directoryPath, new FileSystemScriptOptions(), ScriptType.RunOnce, DbUpDefaults.DefaultRunOrder)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="options">Different options for the file system script provider</param>
        public FileSystemScriptProvider(string directoryPath, FileSystemScriptOptions options) : this(directoryPath, options, ScriptType.RunOnce, DbUpDefaults.DefaultRunOrder)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="options">Different options for the file system script provider</param>
        /// <param name="scriptType">The script type</param>
        public FileSystemScriptProvider(string directoryPath, FileSystemScriptOptions options, ScriptType scriptType) : this(directoryPath, options, scriptType, DbUpDefaults.DefaultRunOrder)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="options">Different options for the file system script provider</param>
        /// <param name="scriptType">The script type</param>
        /// <param name="runOrder">The order group this script will run in</param>
        public FileSystemScriptProvider(string directoryPath, FileSystemScriptOptions options, ScriptType scriptType, int runOrder)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            this.directoryPath = directoryPath;
            this.filter = options.Filter;
            this.encoding = options.Encoding;
            this.options = options;
            this.scriptType = scriptType;
            this.runOrder = runOrder;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var files = Directory.GetFiles(directoryPath, "*.sql", ShouldSearchSubDirectories()).AsEnumerable();
            if (filter != null)
            {
                files = files.Where(filter);
            }
            return files.Select(x => SqlScript.FromFile(directoryPath, x, encoding, scriptType, runOrder))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private SearchOption ShouldSearchSubDirectories()
        {
            return options.IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        }
    }
}
