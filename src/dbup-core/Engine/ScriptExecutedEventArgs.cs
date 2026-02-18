using System;
using DbUp.Engine.Transactions;

namespace DbUp.Engine;

/// <summary>
/// Provides the information for the most recently executed script.
/// </summary>
public class ScriptExecutedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptExecutedEventArgs"/> class.
    /// </summary>
    /// <param name="script">The script that was executed.</param>
    /// <param name="connectionManager">The connection manager used for execution.</param>
    public ScriptExecutedEventArgs(SqlScript script, IConnectionManager connectionManager)
    {
        Script = script;
        ConnectionManager = connectionManager;
    }

    /// <summary>
    /// Returns the connection manager for the script's execution.
    /// </summary>
    public IConnectionManager ConnectionManager { get; private set; }

    /// <summary>
    /// Returns the executed script.
    /// </summary>
    public SqlScript Script { get; private set; }
}
