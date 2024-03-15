using DbUp.Engine.Output;

namespace DbUp.Tests.Engine.Output
{
    public class TraceLoggingTests : BaseLoggingTest
    {
        /// <inheritdoc/>
        protected override IUpgradeLog CreateLogger() => new TraceUpgradeLog();
    }
}
