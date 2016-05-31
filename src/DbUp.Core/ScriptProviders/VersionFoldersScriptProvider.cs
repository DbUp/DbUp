using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.ScriptProviders
{
    ///<summary>
    /// Alternate <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts from version folders in a directory.
    ///</summary>
    public class VersionFoldersScriptProvider : IScriptProvider
    {
        private readonly string directoryPath;
        private readonly Encoding encoding;
        private readonly Func<string, bool> filter;
        private readonly string targetVersion;

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        public VersionFoldersScriptProvider(string directoryPath)
        {
            this.directoryPath = directoryPath;
            this.filter = null;
            this.encoding = Encoding.Default;
            this.targetVersion = null;
        }

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="targetVersion">Exclude scripts in subfolders with a higher version number.</param>
        public VersionFoldersScriptProvider(string directoryPath, string targetVersion)
        {
            this.directoryPath = directoryPath;
            this.filter = null;
            this.encoding = Encoding.Default;
            this.targetVersion = targetVersion;
        }

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="filter">The filter.</param>
        public VersionFoldersScriptProvider(string directoryPath, Func<string, bool> filter)
        {
            this.directoryPath = directoryPath;
            this.filter = filter;
            this.encoding = Encoding.Default;
            this.targetVersion = null;
        }

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="encoding">The encoding.</param>
        ///<param name="targetVersion">Exclude scripts in subfolders with a higher version number.</param>
        public VersionFoldersScriptProvider(string directoryPath, Encoding encoding, string targetVersion)
        {
            this.directoryPath = directoryPath;
            this.filter = null;
            this.encoding = encoding;
            this.targetVersion = targetVersion;
        }

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="encoding">The encoding.</param>
        ///<param name="targetVersion">Exclude scripts in subfolders with a higher version number.</param>
        public VersionFoldersScriptProvider(string directoryPath, Encoding encoding, Func<string, bool> filter)
        {
            this.directoryPath = directoryPath;
            this.filter = filter;
            this.encoding = encoding;
            this.targetVersion = null;
        }

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="filter">The filter.</param>
        ///<param name="targetVersion">Exclude scripts in subfolders with a higher version number.</param>
        public VersionFoldersScriptProvider(string directoryPath, Func<string, bool> filter, string targetVersion)
        {
            this.directoryPath = directoryPath;
            this.filter = filter;
            this.encoding = Encoding.Default;
            this.targetVersion = targetVersion;
        }

        ///<param name="directoryPath">Path to SQL upgrade scripts</param>
        ///<param name="encoding">The encoding.</param>
        ///<param name="filter">The filter.</param>
        ///<param name="targetVersion">Exclude scripts in subfolders with a higher version number.</param>
        public VersionFoldersScriptProvider(string directoryPath, Encoding encoding, Func<string, bool> filter, string targetVersion)
        {
            this.directoryPath = directoryPath;
            this.filter = filter;
            this.encoding = encoding;
            this.targetVersion = targetVersion;
        }

        /// <summary>
        /// Excludes scripts from version folders with a version higher than target version (if any).
        /// Folders are ordered semantically by the version parsed from the folder name.
        /// A <see cref="filter"/> must be supplied for folders to exclude.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when an unparseable folder version is encountered, or when multiple subfolder names parse to the same version number.</exception>
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            var folderNames = Directory.GetDirectories(directoryPath)
                .Select(d => new DirectoryInfo(d).Name);

            // filter on folder names
            if (filter != null)
            {
                folderNames = folderNames.Where(filter);
            }

            var scripts = new List<SqlScript>();

            if (folderNames.Any())
            {
                var filteredFolders = ParseAndFilterFolders(folderNames);

                // Add scripts folder by folder, where folders are sorted semantically
                scripts.AddRange(filteredFolders.Values.SelectMany(folderName => GetScriptsFromFolder(folderName)));
            }

            return scripts;
        }

        private SortedDictionary<Version, string> ParseAndFilterFolders(IEnumerable<string> folderNames)
        {
            Version parsedTargetVersion = string.IsNullOrEmpty(targetVersion) ? null : ParseVersion(targetVersion);

            // Filter folders by target version
            var filteredFolders = new SortedDictionary<Version, string>();
            foreach (var folderName in folderNames)
            {
                // Expecting all encountered folder names to be parseable. 
                var parsedFolderVersion = ParseVersion(folderName);
                if (parsedTargetVersion == null || parsedFolderVersion <= parsedTargetVersion)
                {
                    if (filteredFolders.ContainsKey(parsedFolderVersion))
                    {
                        throw new InvalidOperationException(string.Format("Version '{0}' parsed for folder '{1}' is ambiguous.", parsedFolderVersion, folderName));
                    }

                    filteredFolders.Add(parsedFolderVersion, folderName);
                }
            }

            return filteredFolders;
        }

        /// <summary>
        /// Get scripts from the specified version folder. 
        /// The version folder name is prefixed to the scriptname to make scripts with duplicate names unique when from different folders.
        /// </summary>
        /// <param name="folder">name of subfolder within directory path</param>
        private IEnumerable<SqlScript> GetScriptsFromFolder(string folder)
        {
            var absoluteFolderPath = Path.Combine(directoryPath, folder);

            var folderedFileNames = Directory.GetFiles(absoluteFolderPath, "*.sql")
                .Select(f => Path.Combine(folder, new FileInfo(f).Name))
                .AsEnumerable();

            // filter on folder\file combination
            if (this.filter != null)
            {
                folderedFileNames = folderedFileNames.Where(filter);
            }

            // load file contents
            var sqlScripts = new List<SqlScript>();

            foreach (var folderedFileName in folderedFileNames)
            {
                var filePath = Path.Combine(directoryPath, folderedFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    sqlScripts.Add(SqlScript.FromStream(folderedFileName, fileStream, encoding));
                }
            }

            return sqlScripts;
        }

        private static Version ParseVersion(string str)
        {
            Version parsed;

            if (!TryParseVersion(str, out parsed))
            {
                throw new InvalidOperationException(string.Format("Error parsing version from string '{0}'.", str));
            }
            return parsed;
        }

        private static bool TryParseVersion(string str, out Version parsed)
        {
            // Find at least 1 and max 4 delimited decimals at string start, otherwise fail the entire match.
            var regex = new Regex(@"^(?>0*(\d+)(?:[\^_\-\.,~ ]+0*(\d+))?(?:[\^_\-\.,~ ]+0*(\d+))?(?:[\^_\-\.,~ ]+0*(\d+))?)(?![\^_\-\.,~ ]+\d+)");
            var result = regex.Match(str);

            if (result.Success)
            {
                int val;
                int major = int.Parse(result.Groups[ 1 ].Value);
                int minor = int.TryParse(result.Groups[ 2 ].Value, out val) ? val : 0;
                int build = int.TryParse(result.Groups[ 3 ].Value, out val) ? val : 0;
                int revision = int.TryParse(result.Groups[ 4 ].Value, out val) ? val : 0;

                parsed = new Version(major, minor, build, revision);
            }
            else
            {
                parsed = new Version();
            }

            return result.Success;
        }
    }
}