using Microsoft.Extensions.Logging.Abstractions;

namespace DbUp.Engine.Output;

/// <summary>
/// A logger that does nothing
/// </summary>
public class NoOpUpgradeLog : MicrosoftUpgradeLog
{
    public NoOpUpgradeLog()
        : base(NullLogger.Instance)
    {
            
    }
}
