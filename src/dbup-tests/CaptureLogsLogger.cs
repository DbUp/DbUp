using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Engine.Output;

namespace DbUp.Tests
{
    public class CaptureLogsLogger : IUpgradeLog
    {
        readonly StringBuilder logBuilder = new StringBuilder();
        public List<string> InfoMessages { get; } = new List<string>();
        public List<string> WarnMessages { get; } = new List<string>();
        public List<string> ErrorMessages { get; } = new List<string>();

        public string Log => logBuilder.ToString();

        public void WriteInformation(string format, params object[] args)
        {
            var formattedMsg = string.Format(format, args);
            var value = "Info:         " + formattedMsg;
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
            InfoMessages.Add(formattedMsg);
        }

        public void WriteWarning(string format, params object[] args)
        {
            var formattedValue = string.Format(format, args);
            var value = "Warn:         " + formattedValue;
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
            WarnMessages.Add(formattedValue);
        }

        public void WriteError(string format, params object[] args)
        {
            var formattedMessage = string.Format(format, args);
            var value = "Error:        " + formattedMessage;
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
            ErrorMessages.Add(formattedMessage);
        }

        public void WriteDbOperation(string operation)
        {
            var value = "DB Operation: " + operation;
            Console.WriteLine(value);
            logBuilder.AppendLine(value);
        }
    }
}