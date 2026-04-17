using System;
using System.Collections.Generic;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.ScriptProviders;

class ScriptInstanceProvider : IScriptProvider
{
    readonly IScript[] scripts;
    readonly Func<IScript, string> namer;
    readonly SqlScriptOptions sqlScriptOptions;

    /// <summary>
    /// Provider used to directly include an IScript instance during migrations
    /// </summary>
    /// <param name="scripts">The IScript instances to include</param>
    public ScriptInstanceProvider(params IScript[] scripts)
        : this(GetDefaultNamer(), scripts)
    {
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Default namer uses GetType().FullName which may be trimmed or null in AOT.")]
#endif
    static Func<IScript, string> GetDefaultNamer() => s => s.GetType().FullName + ".cs";

    /// <summary>
    /// Provider used to directly include an IScript instance during migrations
    /// </summary>
    /// <param name="scripts">The IScript instances to include</param>
    /// <param name="namer">A function that returns the name of the script</param>
    public ScriptInstanceProvider(Func<IScript, string> namer, params IScript[] scripts) : this(namer,
        new SqlScriptOptions(), scripts)
    {
    }

    /// <summary>
    /// Provider used to directly include an IScript instance during migrations
    /// </summary>
    /// <param name="scripts">The IScript instances to include</param>
    /// <param name="namer">A function that returns the name of the script</param>
    /// <param name="sqlScriptOptions">The sql script options.</param>
    public ScriptInstanceProvider(Func<IScript, string> namer, SqlScriptOptions sqlScriptOptions,
        params IScript[] scripts)
    {
        this.scripts = scripts;
        this.namer = namer ?? throw new ArgumentNullException(nameof(namer));
        this.sqlScriptOptions = sqlScriptOptions;
    }

    public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
    {
        return scripts
            .Select(s => new LazySqlScript(
                    namer(s),
                    sqlScriptOptions,
                    () => connectionManager.ExecuteCommandsWithManagedConnection(s.ProvideScript)
                )
            )
            .ToArray();
    }
}
