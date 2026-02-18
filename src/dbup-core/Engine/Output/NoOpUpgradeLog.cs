using Microsoft.Extensions.Logging.Abstractions;

namespace DbUp.Engine.Output;

/// <summary>
/// A logger that does nothing
/// </summary>
public class NoOpUpgradeLog : MicrosoftUpgradeLog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoOpUpgradeLog"/> class.
    /// </summary>
    public NoOpUpgradeLog()
        : base(NullLogger.Instance)
    {
            
    }
}
