using System;
using System.IO;
using System.Text;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents a script that comes from some source, e.g. an embedded resource in an assembly. 
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class SqlScript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        public SqlScript(string name, string contents) : this(name, contents, new SqlScriptOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScript"/> class with a specific options - script type and a script order
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="sqlScriptOptions">The script options.</param>        
        public SqlScript(string name, string contents, SqlScriptOptions sqlScriptOptions)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Contents = contents;
            SqlScriptOptions = sqlScriptOptions ?? new SqlScriptOptions();
        }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        /// <value></value>
        public virtual string Contents { get; }

        /// <summary>
        /// Gets the SQL Script Options
        /// </summary>
        public SqlScriptOptions SqlScriptOptions { get; }

        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a file using Default encoding
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path)
        {
            return FromFile(path, DbUpDefaults.DefaultEncoding);
        }

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a file using specified encoding
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path, Encoding encoding) => FromFile(Path.GetDirectoryName(path), path, encoding, new SqlScriptOptions());

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a file using specified encoding
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string basePath, string path, Encoding encoding)
        {
            return FromFile(basePath, path, encoding, new SqlScriptOptions());
        }

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a file using specified encoding and sql script options
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>        
        /// <param name="sqlScriptOptions">The script options</param>        
        /// <returns></returns>
        public static SqlScript FromFile(string basePath, string path, Encoding encoding, SqlScriptOptions sqlScriptOptions)
        {
            var fullPath = Path.GetFullPath(path);
            var fullBasePath = Path.GetFullPath(basePath);

            if (!fullPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The basePath must be a parent of path");

            var filename = fullPath
                .Substring(fullBasePath.Length)
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.')
                .Trim('.');

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return FromStream(filename, fileStream, encoding, sqlScriptOptions);
            }
        }

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a stream using Default encoding
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream)
        {
            return FromStream(scriptName, stream, DbUpDefaults.DefaultEncoding, new SqlScriptOptions());
        }

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a stream using specified encoding
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding)
        {
            return FromStream(scriptName, stream, encoding, new SqlScriptOptions());
        }

        /// <summary>
        /// Create a <see cref="SqlScript"/> from a stream using specified encoding and script options
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>  
        /// <param name="sqlScriptOptions">The script options</param>        
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding, SqlScriptOptions sqlScriptOptions)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                var contents = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, contents, sqlScriptOptions);
            }
        }
    }
}
