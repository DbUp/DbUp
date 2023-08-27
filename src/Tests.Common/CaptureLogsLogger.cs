using System;
using System.Collections.Concurrent;
using System.Text;
using DbUp.Engine.Output;

namespace DbUp.Tests.Common
{
    public class CaptureLogsLogger : IUpgradeLog
    {
        readonly StringBuilder logBuilder = new StringBuilder();
        readonly object padlock = new object();

        public ConcurrentBag<string> TraceMessages { get; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> DebugMessages { get; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> InfoMessages { get; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> WarnMessages { get; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> ErrorMessages { get; } = new ConcurrentBag<string>();

        public string Log => logBuilder.ToString();

        public void LogInformation(string format, params object[] args)
        {
            var formattedMsg = string.Format(format, args);
            var value = "Info:         " + formattedMsg;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
            InfoMessages.Add(formattedMsg);
        }

        public void LogWarning(string format, params object[] args)
        {
            var formattedValue = string.Format(format, args);
            var value = "Warn:         " + formattedValue;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
            WarnMessages.Add(formattedValue);
        }

        public void LogTrace(string format, params object[] args)
        {
            var formattedValue = string.Format(format, args);
            var value = "Trace:         " + formattedValue;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
            TraceMessages.Add(formattedValue);
        }

        public void LogDebug(string format, params object[] args)
        {
            var formattedValue = string.Format(format, args);
            var value = "Debug:         " + formattedValue;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
            DebugMessages.Add(formattedValue);
        }

        public void LogError(string format, params object[] args)
        {
            var formattedMessage = string.Format(format, args);
            var value = "Error:        " + formattedMessage;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
            ErrorMessages.Add(formattedMessage);
        }

        public void LogError(Exception ex, string format, params object[] args)
        {
            var formattedMessage = string.Format(format, args);
            var value = "Error:        " + formattedMessage + " => " + ex.Message;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
            ErrorMessages.Add(formattedMessage);
        }

        public void WriteDbOperation(string operation)
        {
            var value = "DB Operation: " + operation;
            Console.WriteLine(value);
            lock(padlock)
            {
                logBuilder.AppendLine(value);
            }
        }
    }
}
