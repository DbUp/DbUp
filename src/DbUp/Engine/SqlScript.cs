
using System;
using System.Data;
using System.IO;
using DbUp.Builder;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents a SQL Server script that comes from an embedded resource in an assembly. 
    /// </summary>
    public class SqlScript : IScript
    {
        protected string contents;
        protected string name;

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

        public SqlScript()
        {
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
        public virtual string Name
        {
            get { return name; }
        }

        public virtual void Execute(UpgradeConfiguration configuration)
        {
            configuration.SqlScriptExecutor.Execute(this, configuration);
        }

        public static SqlScript FromFile(string path)
        {
            var contents = File.ReadAllText(path);
            var fileName = new FileInfo(path).Name;
            return new SqlScript(fileName, contents);
        }

        public static SqlScript FromStream(string scriptName, Stream stream)
        {
            using (var resourceStreamReader = new StreamReader(stream))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c);
            }
        }
    }
}