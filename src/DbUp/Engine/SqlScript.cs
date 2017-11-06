
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
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        public SqlScript(string name, string contents)
        {
            this.name = name;
            this.contents = contents;
        }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        public virtual string Contents
        {
            get { return contents; }
        }

        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Create a SqlScript from a file using default encoding and default script namer.
        /// </summary>
        /// <param name="path">path to script</param>
        public static SqlScript FromFile(string path)
        {
            return FromFile(path, Encoding.Default);
        }

        /// <summary>
        /// Create a SqlScript from a file using specified encoding and default script namer.
        /// </summary>
        /// <param name="path">path to script</param>
        /// <param name="encoding">Encoding of script content</param>
        public static SqlScript FromFile(string path, Encoding encoding)
        {
            return FromFile(path, encoding, FileSystemScriptNamers.Default().Invoke(path));
        }

        /// <summary>
        /// Create a SqlScript from a file using specified encoding and script name.
        /// </summary>
        /// <param name="path">path to script</param>
        /// <param name="encoding">Encoding of script content</param>
        /// <param name="scriptName">Script name</param>
        public static SqlScript FromFile(string path, Encoding encoding, string scriptName)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return FromStream(scriptName, fileStream, encoding);
            }
        }

        /// <summary>
        /// Create a SqlScript from a stream using Default encoding
        /// </summary>
        /// <param name="scriptName">Script name to use</param>
        /// <param name="stream">Stream to script content</param>
        public static SqlScript FromStream(string scriptName, Stream stream)
        {
            return FromStream(scriptName, stream, Encoding.Default);
        }

        /// <summary>
        /// Create a SqlScript from a stream using specified encoding
        /// </summary>
        /// <param name="scriptName">Script name to use</param>
        /// <param name="stream">Stream to script content</param>
        /// <param name="encoding">Encoding of script content</param>
        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding)
        {
            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c);
            }
        }
    }
}