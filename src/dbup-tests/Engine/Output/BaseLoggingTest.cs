using System;
using DbUp.Engine.Output;

namespace DbUp.Tests.Engine.Output
{
    public abstract class BaseLoggingTest
    {
        /// <summary>
        /// Creates a logger for the test.
        /// </summary>
        /// <returns></returns>
        protected abstract IUpgradeLog CreateLogger();

        [Fact]
        public virtual void LogTrace()
        {
            var logger = this.CreateLogger();

            logger.LogTrace("Test without template placeholders.");
            logger.LogTrace("Logging a {0}.", "Test");
        }

        [Fact]
        public virtual void LogDebug()
        {
            var logger = this.CreateLogger();

            logger.LogDebug("Test without template placeholders.");
            logger.LogDebug("Logging a {0}.", "Test");
        }

        [Fact]
        public virtual void LogInformation()
        {
            var logger = this.CreateLogger();

            logger.LogInformation("Test without template placeholders.");
            logger.LogInformation("Logging a {0}.", "Test");
        }

        [Fact]
        public virtual void LogWarning()
        {
            var logger = this.CreateLogger();

            logger.LogWarning("Test without template placeholders.");
            logger.LogWarning("Logging a {0}.", "Test");
        }

        [Fact]
        public virtual void LogError()
        {
            var logger = this.CreateLogger();

            logger.LogError("Test without template placeholders.");
            logger.LogError("Logging a {0}.", "Test");
        }

        [Fact]
        public virtual void LogError_WithException()
        {
            var logger = this.CreateLogger();
            var ex = new Exception("Level 1 Exception", new Exception("Level 2 Exception", new Exception("Level 3 Exception")));

            logger.LogError(ex, "Test without template placeholders.");
            logger.LogError(ex, "Logging a {0}.", "Test");
        }
    }
}
