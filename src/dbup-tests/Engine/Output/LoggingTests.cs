using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace DbUp.Tests.Engine.Output
{
    public class LoggingTests
    {
#if SUPPORTS_LIBLOG
        [Fact]
        public void WhenNoLoggerIsSpecified_LoggingShouldGoToAutodiscoveredLogger()
        {
            var defaultLogger = Serilog.Log.Logger;
            try
            {
                var capturedLogs = new InMemorySink();
                Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                    .WriteTo.Sink(capturedLogs)
                    .CreateLogger();

                var engine = DeployChanges.To
                    .SQLiteDatabase("Data Source=:memory:")
                    .WithScript(new SqlScript("1234", "SELECT 1"))
                    .JournalTo(new NullJournal())
                    .Build();

                var result = engine.PerformUpgrade();
                result.Successful.ShouldBe(true);
                capturedLogs.Events.ShouldContain(e => e.MessageTemplate.Text == "Executing Database Server script '{0}'");
            }
            finally
            {
                Serilog.Log.Logger = defaultLogger;
            }
        }
#endif


        class InMemorySink : ILogEventSink
        {
            public List<LogEvent> Events { get; } = new List<LogEvent>();
            public void Emit(LogEvent logEvent) => Events.Add(logEvent);
        }
    }
}