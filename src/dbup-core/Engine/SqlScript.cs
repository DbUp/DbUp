
using System;
using System.IO;
using System.Text;
using DbUp.Support;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents a SQL Server script that comes from an embedded resource in an assembly. 
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class SqlScript
    {
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
        public SqlScript(string name, string contents, ScriptType scriptType) : this(name, contents, scriptType, DbUpDefaults.DefaultRunOrder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScript"/> class with a specific script type and a specific order
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="scriptType">The script type.</param>
        /// <param name="runOrder">The run group order</param>
        public SqlScript(string name, string contents, ScriptType scriptType, int runOrder)
        {
            Name = name;
            this.Contents = contents;
            this.ScriptType = scriptType;
            this.runOrder = runOrder;
        }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        /// <value></value>
        public virtual string Contents { get; }

        /// <summary>
        /// Gets the type of script this is.
        /// </summary>
        public ScriptType ScriptType { get; }

        /// <summary>
        /// Gets run group order, by default all scripts run in alphabetical order, use this when you want to specify a group of scripts to run first or last
        /// </summary>
        public int runOrder { get; set; }

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
        public static SqlScript FromFile(string path, Encoding encoding) => FromFile(Path.GetDirectoryName(path), path, encoding, ScriptType.RunOnce, DbUpDefaults.DefaultRunOrder);

        /// <summary>
        /// Create a SqlScript from a file using specified encoding
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string basePath, string path, Encoding encoding)
        {
            return FromFile(basePath, path, encoding, ScriptType.RunOnce, DbUpDefaults.DefaultRunOrder);
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
            return FromFile(basePath, path, encoding, scriptType, DbUpDefaults.DefaultRunOrder);
        }

        /// <summary>
        /// Create a SqlScript from a file using specified encoding and script type and run order
        /// </summary>
        /// <param name="basePath">Root path that was searched</param>
        /// <param name="path">Path to the file</param>
        /// <param name="encoding"></param>        
        /// <param name="scriptType">The script type</param>
        /// <param name="runOrder">The run group order</param>
        /// <returns></returns>
        public static SqlScript FromFile(string basePath, string path, Encoding encoding, ScriptType scriptType, int runOrder)
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

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return FromStream(filename, fileStream, encoding, scriptType, runOrder);
            }
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
            return FromStream(scriptName, stream, encoding, scriptType, DbUpDefaults.DefaultRunOrder);
        }

        /// <summary>
        /// Create a SqlScript from a stream using specified encoding and script type and run group order
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>  
        /// <param name="scriptType">The script type</param>
        /// <param name="runOrder">The run order</param>
        /// <returns></returns>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding, ScriptType scriptType, int runOrder)
        {
            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c, scriptType, runOrder);
            }
        }
    }
}