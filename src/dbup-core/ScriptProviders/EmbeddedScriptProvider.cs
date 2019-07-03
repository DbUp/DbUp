using System;
using System.Reflection;
using System.Text;
using DbUp.Engine;
using DbUp.Support;

namespace DbUp.ScriptProviders
{
    /// <summary>
    /// The default <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts embedded in an assembly.
    /// </summary>
    public class EmbeddedScriptProvider : EmbeddedScriptsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="hasher">The hasher.</param>
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter, IHasher hasher) : this(assembly, filter, DbUpDefaults.DefaultEncoding, new SqlScriptOptions(), hasher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="hasher">The hasher.</param>
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter, Encoding encoding, IHasher hasher) : this(assembly, filter, encoding, new SqlScriptOptions(), hasher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="sqlScriptOptions">The sql script options</param>
        /// <param name="hasher">The hasher.</param>
        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter, Encoding encoding, SqlScriptOptions sqlScriptOptions, IHasher hasher) : base(new[] { assembly }, filter, encoding, sqlScriptOptions, hasher)
        {
        }
    }
}