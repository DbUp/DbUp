using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using DbUp.Engine.Output;

namespace DbUp.Tests.Common;

public class CaptureLogsLogger : IUpgradeLog
{
    readonly StringBuilder logBuilder = new();

    public ConcurrentBag<string> TraceMessages { get; } = new();
    public ConcurrentBag<string> DebugMessages { get; } = new();
    public ConcurrentBag<string> InfoMessages { get; } = new();
    public ConcurrentBag<string> WarnMessages { get; } = new();
    public ConcurrentBag<string> ErrorMessages { get; } = new();
    public ConcurrentBag<string> WriteDbOperations { get; } = new();

    public string Log
    {
        get
        {
            lock (logBuilder)
                return logBuilder.ToString();
        }
    }


    public void LogInformation(string format, params object[] args)
    {
        var formattedMsg = string.Format(format, args);
        var value = "Info:         " + formattedMsg;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        InfoMessages.Add(formattedMsg);
    }

    public void LogWarning(string format, params object[] args)
    {
        var formattedValue = string.Format(format, args);
        var value = "Warn:         " + formattedValue;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        WarnMessages.Add(formattedValue);
    }

    public void LogTrace(string format, params object[] args)
    {
        var formattedValue = string.Format(format, args);
        var value = "Trace:         " + formattedValue;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        TraceMessages.Add(formattedValue);
    }

    public void LogDebug(string format, params object[] args)
    {
        var formattedValue = string.Format(format, args);
        var value = "Debug:         " + formattedValue;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        DebugMessages.Add(formattedValue);
    }

    public void LogError(string format, params object[] args)
    {
        var formattedMessage = string.Format(format, args);

        // Remove stack trace information
        formattedMessage = string.Join(
            "\n",
            formattedMessage.Split('\n').Where(l => !l.StartsWith("   at "))
        ).Trim();

        var value = "Error:        " + formattedMessage;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        ErrorMessages.Add(formattedMessage);
    }

    public void LogError(Exception ex, string format, params object[] args)
    {
        var formattedMessage = string.Format(format, args);
        // Remove stack trace information
        formattedMessage = string.Join(
            "\n",
            formattedMessage.Split('\n').Where(l => !l.StartsWith("   at "))
        ).Trim();

        var value = "Error:        " + formattedMessage + " => " + ex.Message;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        ErrorMessages.Add(formattedMessage);
    }

    public void WriteDbOperation(string operation)
    {
        var value = "DB Operation: " + operation;
        Console.WriteLine(value);
        lock (logBuilder)
            logBuilder.AppendLine(value);

        WriteDbOperations.Add(operation);
    }
}
