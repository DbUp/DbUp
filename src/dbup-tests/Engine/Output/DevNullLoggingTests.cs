using DbUp.Engine.Output;

namespace DbUp.Tests.Engine.Output
{
    public class DevNullLoggingTests : BaseLoggingTest
    {
        /// <inheritdoc/>
        protected override IUpgradeLog CreateLogger() => MicrosoftUpgradeLog.DevNull;
    }
}
