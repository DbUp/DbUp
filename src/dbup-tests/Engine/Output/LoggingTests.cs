using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

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

            var engine = DeployChanges.To
                .SQLiteDatabase("Data Source=:memory:")
                .WithScript(new SqlScript("1234", "SELECT 1"))
                .JournalTo(new NullJournal())
                .LogTo(factory)
                .Build();

            var result = engine.PerformUpgrade();
            result.Successful.ShouldBe(true);
            capturedLogs.Events.ShouldContain(e => e.MessageTemplate.Text == "Executing Database Server script '{0}'");
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

            var engine = DeployChanges.To
                .SQLiteDatabase("Data Source=:memory:")
                .WithScript(new SqlScript("1234", "SELECT 1"))
                .JournalTo(new NullJournal())
                .LogTo(logger)
                .Build();

            var result = engine.PerformUpgrade();

            result.Successful.ShouldBe(true);
            capturedLogs.Events.ShouldContain(e => e.MessageTemplate.Text == "Executing Database Server script '{0}'");
        }

    class InMemorySink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = new();
        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
    }
}
