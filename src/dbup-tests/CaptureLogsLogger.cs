using System;
using System.Diagnostics;
using System.Text;
using DbUp.Engine.Output;

namespace DbUp.Tests
{
    public class CaptureLogsLogger : IUpgradeLog
    {
        readonly StringBuilder logBuilder = new StringBuilder();

        public string Log => logBuilder.ToString();

        public void WriteInformation(string format, params object[] args)
        {
            var value = "Info:         " + string.Format(format, args);
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
        }

        public void WriteError(string format, params object[] args)
        {
            var value = "Error:        " + string.Format(format, args);
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
        }

        public void WriteWarning(string format, params object[] args)
        {
            var value = "Warn:         " + string.Format(format, args);
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
        }

        public void WriteDbOperation(string operation)
        {
            var value = "DB Operation: " + operation;
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
        }
    }
}