using DbUp.Engine.Output;

namespace DbUp.Tests.Engine.Output
{
    public class AggregateLoggingTests : BaseLoggingTest
    {
        /// <inheritdoc/>
        protected override IUpgradeLog CreateLogger() 
            => new AggregateLog(new IUpgradeLog[] { new ConsoleUpgradeLog(), new TraceUpgradeLog(), MicrosoftUpgradeLog.DevNull });
    }
}
