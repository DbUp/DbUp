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

        private readonly Func<string, string> hashProvider;
        private string hash;


        /// <summary>
        /// Initializes a new instance of the <see cref="LazySqlScript" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contentProvider">The delegate which creates the content at execution time.</param>
        /// <param name="hashProvider">The hash provider.</param>
        public LazySqlScript(string name, Func<string> contentProvider, Func<string, string> hashProvider)
            : this(name, new SqlScriptOptions(), contentProvider, hashProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySqlScript" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlScriptOptions">The sql script options.</param>
        /// <param name="contentProvider">The delegate which creates the content at execution time.</param>
        /// <param name="hashProvider">The hash provider.</param>
        public LazySqlScript(string name, SqlScriptOptions sqlScriptOptions, Func<string> contentProvider, Func<string, string> hashProvider)
            : base(name, null, null, sqlScriptOptions)
        {
            this.contentProvider = contentProvider;
            this.hashProvider = hashProvider;
        }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        /// <value></value>
        public override string Contents => content ?? (content = contentProvider());

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public override string Hash => hash ?? (hash = hashProvider(Contents));
    }
}