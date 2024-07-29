using DbUp.Engine.Output;

namespace DbUp.Tests.Engine.Output
{
    public class ConsoleLoggingTests : BaseLoggingTest
    {
        /// <inheritdoc/>
        protected override IUpgradeLog CreateLogger() => new ConsoleUpgradeLog();
    }
}
