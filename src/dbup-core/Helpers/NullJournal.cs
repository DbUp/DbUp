using System;
using System.Data;
using DbUp.Engine;

namespace DbUp.Helpers;

/// <summary>
/// Enables multiple executions of idempotent scripts.
/// </summary>
public class NullJournal : IJournal
{
    /// <summary>
    /// Returns an empty array of length 0.
    /// </summary>
    /// <returns></returns>
    public string[] GetExecutedScripts() => new string[0];

    /// <summary>
    /// Does not store the script, simply returns.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="dbCommandFactory"></param>
    public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
    {
    }

    /// <summary>
    /// Does not ensure table exists, simply returns.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="dbCommandFactory"></param>
    public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
    {
    }
}
