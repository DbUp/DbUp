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
        readonly string directoryPath;
        readonly Func<string, bool> filter;
        readonly Encoding encoding;
        readonly FileSystemScriptOptions options;
        readonly SqlScriptOptions sqlScriptOptions;

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
            filter = options.Filter;
            encoding = options.Encoding;
            this.options = options;
            this.sqlScriptOptions = sqlScriptOptions;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            if (!options.UseOnlyFilenameForScriptName)
            {
                var files = new List<string>();
                foreach (var scriptExtension in options.Extensions)
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
            else
            {
                var files = new List<FileInfo>();
                foreach (var scriptExtension in options.Extensions)
                {
                    files.AddRange(
                        new DirectoryInfo(directoryPath).GetFiles(scriptExtension, ShouldSearchSubDirectories())
                    );
                }
                var dupeFiles = files.GroupBy(f => f.Name).Where(grp => grp.Count() > 1).SelectMany(f => f.Select(fi => fi.FullName)).ToList();
                if (dupeFiles.Count > 0) {
                    var sbError = new StringBuilder();
                    sbError.AppendLine("Duplicate filenames:");
                    foreach (var file in dupeFiles) {
                        sbError.AppendLine($"- {file}");
                    }
                    throw new Exception(sbError.ToString());
                }
                if (filter != null)
                {
                    files = files.Where(x => filter(x.Name)).ToList();
                }
                return files.Select(x => SqlScript.FromStream(x.Name, new FileStream(x.FullName, FileMode.Open, FileAccess.Read), encoding, sqlScriptOptions))
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        SearchOption ShouldSearchSubDirectories()
        {
            return options.IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        }
    }
}
