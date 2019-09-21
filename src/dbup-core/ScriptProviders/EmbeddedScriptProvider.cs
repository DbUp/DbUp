using System;
using System.Reflection;
using System.Text;
using DbUp.Engine;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// The default <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts embedded in an assembly.
    /// </summary>
    public class EmbeddedScriptProvider : EmbeddedScriptsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter) : this(assembly, filter, DbUpDefaults.DefaultEncoding, new SqlScriptOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter, Encoding encoding) : this(assembly, filter, encoding, new SqlScriptOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="sqlScriptOptions">The sql script options</param>        
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter, Encoding encoding, SqlScriptOptions sqlScriptOptions) : base(new[] { assembly ?? throw new ArgumentNullException(nameof(assembly)) }, filter, encoding, sqlScriptOptions)
        {
        }
    }
}
