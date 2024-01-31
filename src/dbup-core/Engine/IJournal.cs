using System;
using System.Data;

namespace DbUp.Engine;

/// <summary>
/// This interface is provided to allow different projects to store version information differently.
/// </summary>
public interface IJournal
{
    /// <summary>
    /// Recalls the version number of the database.
    /// </summary>
    /// <returns></returns>
    string[] GetExecutedScripts();

    /// <summary>
    /// Records an upgrade script for a database.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <param name="dbCommandFactory"></param>
    void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory);

    /// <summary>
    /// Creates the journal if it does not exist, and if it does exist makes sure it is in the latest format
    /// This is called just before a script is executed
    /// </summary>
    void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory);
}
