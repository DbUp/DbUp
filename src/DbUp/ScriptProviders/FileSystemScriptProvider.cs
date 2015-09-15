using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private string directoryPath;

        private string DirectoryPath
        {
            get { return directoryPath; }
            set
            {
                directoryPath =
                    string.IsNullOrEmpty(value)
                        ? string.Empty
                        : string.Concat
                            (
                                value.Trim(),
                                value.Substring(value.Length - 1) != "\\"
                                    ? "\\"
                                    : string.Empty
                            );
            }               
            
        }

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
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            if (string.IsNullOrEmpty(DirectoryPath.Trim()))
            {
                DirectoryPath = Environment.CurrentDirectory;
            }

            //  Notes:  
            //      1.  When Directory.Getfiles() is passed an extension that is EXACTLY 3 characters in length, the 
            //          resulting file list will contain any file with an extension longer than 4 characters that
            //          begins with the specified extension. This is a documented FEATURE. 
            //      2.  In order to limit the returned file list to just those with the extension ".sql" (nothing 
            //          more and nothing less, it is necessary to add a .Where condition that filters on the 
            //          extension.
            //      Why do both? To limit the list returned by Directory.Getfiles() in case there are lots of other
            //      files in the specified DirectoryPath...
            var searchOptions = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return Directory
                .GetFiles(DirectoryPath,"*.sql" /* see Note(1) above.*/, searchOptions)
                .Where(FileExtensionIsDotSql) /* see Note(2) above */
                .Where(Filter)
                .Select(SqlScriptFromFile)
                .ToList();
        }

        private bool FileExtensionIsDotSql(string filepath)
        {
            return Path.HasExtension(filepath) &&
                   ".sql".Equals(Path.GetExtension(filepath), StringComparison.InvariantCultureIgnoreCase);
        }

        private SqlScript SqlScriptFromFile(string filePath)
        {
            // Get the base folder name from the DirectoryPath
            var baseFolderName = DirectoryPath.Split('\\').LastOrDefault(s => !string.IsNullOrEmpty(s));
            // Get the filename relative to the base folder
            var filePathRelativeToBaseFolder = filePath.Replace(DirectoryPath, string.Empty);

            // construct the value for the SqlScript.Name property 
            var filePathIncludingBaseFolder = string.Concat(baseFolderName, "\\", filePathRelativeToBaseFolder);

            return SqlScript.FromStream(filePathIncludingBaseFolder, new FileStream(filePath, FileMode.Open, FileAccess.Read), Encoding);
        }
    }
}
