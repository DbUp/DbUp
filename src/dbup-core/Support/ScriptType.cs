namespace DbUp.Support;

/// <summary>
/// Defines how a script should be executed.
/// </summary>
public enum ScriptType
{
    /// <summary>
    /// Default script type.  The script will run only once.
    /// </summary>
    RunOnce = 0,

    /// <summary>
    /// The script will not be journaled and always be run.  Useful for setting permissions, or deploying stored procedures or UDFs. Please note, the script should be written so it can always be ran.
    /// </summary>
    RunAlways = 1
}
