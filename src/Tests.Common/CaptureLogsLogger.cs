using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbUp.Engine.Output;

namespace DbUp.Tests.Common;

public class CaptureLogsLogger : IUpgradeLog
{
    readonly StringBuilder logBuilder = new();
    public List<string> InfoMessages { get; } = new();
    public List<string> WarnMessages { get; } = new();
    public List<string> ErrorMessages { get; } = new();
    public List<string> WriteDbOperations { get; } = new();

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

        // Remove stack trace information
        formattedMessage = string.Join(
            "\n",
            formattedMessage.Split('\n').Where(l => !l.StartsWith("   at "))
        ).Trim();

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
        WriteDbOperations.Add(operation);
    }
}
