using System;
using DbUp.Support;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents a SQL Server script that is fetched at execution time, rather than discovery time
    /// </summary>
    public class LazySqlScript : SqlScript
    {
        private readonly Func<string> contentProvider;
        private string content;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contentProvider">The delegate which creates the content at execution time.</param>
        public LazySqlScript(string name, Func<string> contentProvider)
            : this(name, ScriptType.RunOnce, DbUpDefaults.DefaultRunOrder, null)
        {
            this.contentProvider = contentProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="scriptType">The script type.</param>        
        /// <param name="contentProvider">The delegate which creates the content at execution time.</param>
        public LazySqlScript(string name, ScriptType scriptType, Func<string> contentProvider)
            : this(name, scriptType, DbUpDefaults.DefaultRunOrder, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySqlScript"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="scriptType">The script type.</param>
        /// <param name="runOrder">The run order this script will be ran in.</param>
        /// <param name="contentProvider">The delegate which creates the content at execution time.</param>
        public LazySqlScript(string name, ScriptType scriptType, int runOrder, Func<string> contentProvider)
            : base(name, null, scriptType, runOrder)
        {
            this.contentProvider = contentProvider;
        }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        /// <value></value>
        public override string Contents
        {
            get { return content ?? (content = contentProvider()); }
        }
    }
}