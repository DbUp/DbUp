using System;
using System.Text;
using DbUp.Engine.Output;

namespace DbUp.Tests
{
    public class CaptureLogsLogger : IUpgradeLog
    {
        readonly StringBuilder logBuilder = new StringBuilder();

        public string Log { get { return logBuilder.ToString(); } }

        public void WriteInformation(string format, params object[] args)
        {
            logBuilder.AppendLine("Info:         " + string.Format(format, args));
        }

        public void WriteError(string format, params object[] args)
        {
            logBuilder.AppendLine("Error:        " + string.Format(format, args));
        }

        public void WriteWarning(string format, params object[] args)
        {
            logBuilder.AppendLine("Warn:         " + string.Format(format, args));
        }

        public void WriteDbOperation(string operation)
        {
            logBuilder.AppendLine("DB Operation: " + operation);
        }
    }
}