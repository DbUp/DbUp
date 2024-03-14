using DbUp.Tests.TestInfrastructure;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;

namespace DbUp.Tests.Engine.Output;

public class LoggingTests
{
    [Fact]
    public void LogTo_Accepts_ILoggerFactory()
    {
        var capturedLogs = new InMemorySink();

        var factory = new LoggerFactory().AddSerilog(
            new LoggerConfiguration()
                .WriteTo.Sink(capturedLogs)
                .CreateLogger()
        );

        var testProvider = new TestProvider();
        var engine = testProvider.Builder
            .WithScript(new SqlScript("1234", "SELECT 1"))
            .LogTo(factory)
            .Build();

        var result = engine.PerformUpgrade();
        result.Successful.ShouldBe(true);
        capturedLogs.LogEvents.ShouldContain(e => e.MessageTemplate.Text == "Executing Database Server script '{0}'");
    }

    [Fact]
    public void LogTo_Accepts_ILogger()
    {
        var capturedLogs = new InMemorySink();

        var factory = new LoggerFactory().AddSerilog(
            new LoggerConfiguration()
                .WriteTo.Sink(capturedLogs)
                .CreateLogger()
        );

        var logger = factory.CreateLogger<LoggingTests>();

        var testProvider = new TestProvider();
        var engine = testProvider.Builder
            .WithScript(new SqlScript("1234", "SELECT 1"))
            .LogTo(logger)
            .Build();

        var result = engine.PerformUpgrade();

        result.Successful.ShouldBe(true);
        capturedLogs.LogEvents.ShouldContain(e => e.MessageTemplate.Text == "Executing Database Server script '{0}'");
    }
}
