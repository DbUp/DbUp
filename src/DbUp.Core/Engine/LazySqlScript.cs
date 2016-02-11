using System;

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
            : base(name, null)
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