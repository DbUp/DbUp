using System;
using System.Text;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// Options for the <see cref="FileSystemScriptProvider"/>
    /// </summary>
    public class FileSystemScriptOptions
    {
        /// <summary>
        /// Create new options for the <see cref="FileSystemScriptProvider"/>
        /// </summary>
        public FileSystemScriptOptions()
        {
            Encoding = Encoding.Default;
        }
        /// <summary>
        /// The provider will look in subdirectories for scripts files.
        /// </summary>
        public bool IncludeSubDirectories { get; set; }

        /// <summary>
        /// The filter to be used for filtering files 
        /// <remarks> Files which does not end by .sql are never considered </remarks>
        /// </summary>
        public Func<string, bool> Filter { get; set; }

        /// <summary>
        /// The encoding to be used for reading files 
        /// </summary>
        public Encoding Encoding { get; set; }
    }
}