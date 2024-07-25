using System;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Tests.Common;

namespace DbUp.Tests.Builder;

public class UpgradeConfigurationFixture
{
    [Fact]
    public void Default_Logger_Has_No_Providers()
    {
        var config = new UpgradeConfiguration();

        config.Log.ShouldNotBeNull();
        config.Log.ShouldBeOfType<AggregateLog>();
        config.Log.HasLoggers.ShouldBeFalse();
        config.Log.LoggerCount.ShouldBe(0);
    }

    [Fact]
    public void Adding_Logger_Increments_Providers()
    {
        var config = new UpgradeConfiguration();
        config.AddLog(new NoOpUpgradeLog());

        config.Log.HasLoggers.ShouldBeTrue();
        config.Log.LoggerCount.ShouldBe(1);
    }

    [Fact]
    public void Logger_Writes_To_All_Providers()
    {
        var log1 = new CaptureLogsLogger();
        var log2 = new CaptureLogsLogger();
        var log3 = new CaptureLogsLogger();

        var config = new UpgradeConfiguration();

        config.AddLog(log1);
        config.AddLog(log2);
        config.AddLog(log3);

        config.Log.HasLoggers.ShouldBeTrue();
        config.Log.LoggerCount.ShouldBe(3);

        config.Log.LogInformation("Test");

        log1.InfoMessages.ShouldContain("Test");
        log2.InfoMessages.ShouldContain("Test");
        log3.InfoMessages.ShouldContain("Test");
    }

    class TestLog : IUpgradeLog
    {
        public bool WasWritten { get; private set; }

        public void LogInformation(string format, params object[] args) => WasWritten = true;

        public void LogError(string format, params object[] args) => WasWritten = true;

        public void LogWarning(string format, params object[] args) => WasWritten = true;

        public void LogTrace(string format, params object[] args) => WasWritten = true;

        public void LogDebug(string format, params object[] args) => WasWritten = true;

        public void LogError(Exception ex, string format, params object[] args) => WasWritten = true;
    }
}
