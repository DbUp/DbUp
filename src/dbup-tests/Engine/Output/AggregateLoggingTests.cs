using System;
using DbUp.Engine.Output;

namespace DbUp.Tests.Engine.Output
{
    public class AggregateLoggingTests : BaseLoggingTest
    {
        /// <inheritdoc/>
        protected override IUpgradeLog CreateLogger()
            => new AggregateLog(new IUpgradeLog[] {new ConsoleUpgradeLog(), new TraceUpgradeLog(), new NoOpUpgradeLog()});

        [Fact]
        public void Logs_Silently_When_No_Loggers_Are_Added()
        {
            var logger = new AggregateLog();

            logger.LogTrace("Test");
            logger.LogDebug("Test");
            logger.LogInformation("Test");
            logger.LogWarning("Test");
            logger.LogError("Test");
            logger.LogError(new Exception("Test Exception"), "Test");
        }
    }
}
