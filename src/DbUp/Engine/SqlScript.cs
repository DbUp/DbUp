
using System;
using System.IO;
using System.Security.Cryptography;
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
        /// <value></value>
        public virtual string Contents
        {
            get { return contents; }
        }

        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets a SHA2 Hash for the script content
        /// </summary>
        public string Hash
        {
            get
            {
                var sha = new SHA256Managed();
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(this.contents));
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// Create a SqlScript from a file using Default encoding
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path)
        {
            return FromFile(path, Encoding.Default);
        }

        /// <summary>
        /// Create a SqlScript from a file using specified encoding
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path, Encoding encoding)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var fileName = new FileInfo(path).Name;
                return FromStream(fileName, fileStream, encoding);
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
            return FromStream(scriptName, stream, Encoding.Default);
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
            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c);
            }
        }
    }
}