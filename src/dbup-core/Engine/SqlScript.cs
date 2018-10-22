
using System;
using System.IO;
using System.Text;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents a SQL Server script that comes from an embedded resource in an assembly. 
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class SqlScript
    {
        private readonly string contents;        

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        public SqlScript(string name, string contents) : this(name, contents, ScriptType.RunOnce)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="scriptType">The script type.</param>
        public SqlScript(string name, string contents, ScriptType scriptType)
        {
            Name = name;
            this.contents = contents;
            this.ScriptType = ScriptType;
        }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        /// <value></value>
        public virtual string Contents
        {
            get { return contents; }
        }

        public ScriptType ScriptType { get; }

        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        /// <value></value>
        public string Name { get; }

        /// <summary>
        /// Create a SqlScript from a file using Default encoding
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path)
        {
            return FromFile(path, DbUpDefaults.DefaultEncoding);
        }

        /// <summary>
        /// Create a SqlScript from a file using specified encoding
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path, Encoding encoding)
            => FromFile(Path.GetDirectoryName(path), path, encoding);
        
        /// <summary>
        /// Create a SqlScript from a file using specified encoding
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string basePath, string path, Encoding encoding)
        {
            return FromFile(basePath, path, encoding, ScriptType.RunOnce);
        }

        /// <summary>
        /// Create a SqlScript from a file using specified encoding and script type
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>        
        /// <param name="scriptType">The script type</param>
        /// <returns></returns>
        public static SqlScript FromFile(string basePath, string path, Encoding encoding, ScriptType scriptType)
        {
            var fullPath = Path.GetFullPath(path);
            var fullBasePath = Path.GetFullPath(basePath);

            if (!fullPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
                throw new Exception("The basePath must be a parent of path");

            var filename = fullPath
                .Substring(fullBasePath.Length)
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.')
                .Trim('.');

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return FromStream(filename, fileStream, encoding, scriptType);
        }

        /// <summary>
        /// Create a SqlScript from a stream using Default encoding
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream)
        {
            return FromStream(scriptName, stream, DbUpDefaults.DefaultEncoding);
        }

        /// <summary>
        /// Create a SqlScript from a stream using specified encoding
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding)
        {
            return FromStream(scriptName, stream, encoding, ScriptType.RunOnce);
        }

        /// <summary>
        /// Create a SqlScript from a stream using specified encoding and script type
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>  
        /// <param name="scriptType">The script type</param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding, ScriptType scriptType)
        {
            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c, scriptType);
            }
        }
    }
}