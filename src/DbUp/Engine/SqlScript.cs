
using System;
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
        static readonly Object locker = new object();

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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SqlScript FromFile(string path)
        {
            lock (locker)
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var fileName = new FileInfo(path).Name;
                    return FromStream(fileName, fileStream);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
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