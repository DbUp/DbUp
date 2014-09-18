using System.IO;
using System.Text;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents a SQL Server script that comes from an embedded resource in an assembly. 
    /// </summary>
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
        /// Create SqlScript from file on location
        /// </summary>
        /// <param name="path">Full path location of file.</param>
        /// <returns>New SqlScript instance from file.</returns>
        public static SqlScript FromFile(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var fileName = new FileInfo(path).Name;
                return FromStream(fileName, fileStream);
            }
        }

        /// <summary>
        /// Create SqlScript from Stream
        /// </summary>
        /// <param name="scriptName">Name of script to get from stream</param>
        /// <param name="stream">SqlScript stream</param>
        /// <returns>New SqlScript instance from stream.</returns>
        public static SqlScript FromStream(string scriptName, Stream stream)
        {
            using (var resourceStreamReader = new StreamReader(stream, Encoding.Default, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c);
            }
        }
    }
}