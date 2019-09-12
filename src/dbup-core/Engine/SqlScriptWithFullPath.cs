using System;
using System.IO;
using System.Text;

namespace DbUp.Engine
{
    public class SqlScriptWithFullPath : SqlScript
    {
        public string FullPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptWithFullPath"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="fullPath">The script's full path.</param>
        public SqlScriptWithFullPath(string name, string contents, string fullPath) : base(name, contents)
        {
            FullPath = fullPath;
        }

        /// <summary>
        /// Create a SqlScriptWithFullPath from a file using specified encoding
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFileWithFullPath(string basePath, string path, Encoding encoding)
        {
            string fullPath = Path.GetFullPath(path);
            string fullBasePath = Path.GetFullPath(basePath);

            if (!fullPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The basePath must be a parent of path");
            }

            string filename = fullPath
                .Substring(fullBasePath.Length)
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.')
                .Trim('.');

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return FromStream(filename, fileStream, encoding, fullPath);
            }
        }

        /// <summary>
        /// Create a SqlScript from a stream using Default encoding
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, string fullPath)
        {
            return FromStream(scriptName, stream, DbUpDefaults.DefaultEncoding, fullPath);
        }

        /// <summary>
        /// Create a SqlScript from a stream using specified encoding
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding, string fullPath)
        {
            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScriptWithFullPath(scriptName, c, fullPath);
            }
        }
    }
}
