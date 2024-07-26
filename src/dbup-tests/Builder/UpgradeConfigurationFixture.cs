using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Tests.Common;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Builder;

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
        var log1 = new CaptureLogsLogger();
        var log2 = new CaptureLogsLogger();
        var log3 = new CaptureLogsLogger();

        var config = new UpgradeConfiguration();
        config.AddLog(log1);
        config.AddLog(log2);
        config.AddLog(log3);
        config.Log.WriteInformation("Test");

        config.Log.ShouldBeOfType<MultipleUpgradeLog>();
        log1.InfoMessages.ShouldContain("Test");
        log2.InfoMessages.ShouldContain("Test");
        log3.InfoMessages.ShouldContain("Test");
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
}
