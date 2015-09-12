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
        private Encoding encoding;
        private Func<string, bool> filter;

        private string DirectoryPath { get; set; }

        private Func<string, bool> Filter
        {
            get { return filter ?? (s => true); }
            set { filter = value; }
            
        }
        private Encoding Encoding 
        { 
            get { return encoding ?? Encoding.Default; }
            set { encoding = value; }
        }

        private bool Recursive { get; set; }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        public FileSystemScriptProvider(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Filter = null;
            Encoding = Encoding.Default;
            Recursive = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="recursive">Include sub-folders?</param>
        public FileSystemScriptProvider(string directoryPath, bool recursive)
        {
            DirectoryPath = directoryPath;
            Filter = null;
            Encoding = Encoding.Default;
            Recursive = recursive;
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="filter">The filter.</param>
        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter)
        {
            DirectoryPath = directoryPath;
            Filter = filter;
            Encoding = Encoding.Default;
            Recursive = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="filter">The filter.</param>
        /// <param name="recursive">Include sub-folders?</param>
        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter, bool recursive)
        {
            DirectoryPath = directoryPath;
            Filter = filter;
            Encoding = Encoding.Default;
            Recursive = recursive;
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="encoding">The encoding.</param>
        public FileSystemScriptProvider(string directoryPath, Encoding encoding)
        {
            DirectoryPath = directoryPath;
            Filter = null;
            Encoding = encoding;
            Recursive = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="recursive">Include sub-folders?</param>
        public FileSystemScriptProvider(string directoryPath, Encoding encoding, bool recursive)
        {
            DirectoryPath = directoryPath;
            Filter = null;
            Encoding = encoding;
            Recursive = recursive;
        }

        ///<summary>
        ///</summary>
        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="filter">The filter.</param>
        ///<param name="encoding">The encoding.</param>
        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter, Encoding encoding)
        {
            DirectoryPath = directoryPath;
            Filter = filter;
            Encoding = encoding;
            Recursive = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath">Path to SQL upgrade scripts</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="recursive">Include sub-folders?</param>
        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter, Encoding encoding, bool recursive)
        {
            DirectoryPath = directoryPath;
            Filter = filter;
            Encoding = encoding;
            Recursive = recursive;
        }

        /// <summary>
        /// Gets all scripts that should be executed.
        /// </summary>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return
                Directory.GetFiles
                    (
                        DirectoryPath,
                        "*.sql", // According to docs, this is actually "*.sql*". Workaround below
                        Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                    )
                    .Where(p => Path.HasExtension(p) && Path.GetExtension(p) == ".sql") // Filter out "a.sql1", "a.sql12", "a.sql123", etc.
                    .Where(Filter)
                    .Select(p => SqlScript.FromStream(p.Replace(string.Concat(DirectoryPath, "\\"), string.Empty),
                        new FileStream(p, FileMode.Open, FileAccess.Read),
                        Encoding))
                    .ToList();


        }
    }
}
