using DbUp.Builder;
using DbUp.Engine.Output;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Builder
{
    public class UpgradeConfigurationFixture
    {
        [Fact]
        public void WhenNoLoggerIsAddedThenTheDefaultLoggerIsReturned()
        {
            new UpgradeConfiguration()
                .Log.ShouldNotBeNull();
        }

        [Fact]
        public void WhenASingleLoggerIsAddedThenItselfShouldBeReturned()
        {
            var config = new UpgradeConfiguration();
            var addedLog = new NoOpUpgradeLog();
            config.AddLog(addedLog);
            config.Log.ShouldBe(addedLog);
        }

        [Fact]
        public void WhenMultipleLoggersAreAddedThenAMultipleLoggerShouldBeReturnedAndLogsGoToAllDestinations()
        {
            var log1 = new TestLog();
            var log2 = new TestLog();
            var log3 = new TestLog();

            var config = new UpgradeConfiguration();
            config.AddLog(log1);
            config.AddLog(log2);
            config.AddLog(log3);
            config.Log.WriteInformation("Test");

            config.Log.ShouldBeOfType<MultipleUpgradeLog>();
            log1.WasWritten.ShouldBe(true);
            log2.WasWritten.ShouldBe(true);
            log3.WasWritten.ShouldBe(true);
        }

        [Fact]
        public void WhenTheLoggerIsClearedThenTheDefaultLoggerReturns()
        {
            var config = new UpgradeConfiguration();
            var defaultLog = config.Log;
            config.AddLog(new NoOpUpgradeLog());
            config.Log.ShouldNotBe(defaultLog);

            config.Log = null;
            config.Log.ShouldBe(defaultLog);
        }

        class TestLog : IUpgradeLog
        {
            public bool WasWritten { get; private set; }
            public void WriteInformation(string format, params object[] args)
            {
                WasWritten = true;
            }

            public void WriteError(string format, params object[] args)
            {
                WasWritten = true;
            }

            public void WriteWarning(string format, params object[] args)
            {
                WasWritten = true;
            }
        }
    }
}