using System;

namespace DbUp.Engine.Output;

/// <summary>
/// A log that writes to the console in a colorful way.
/// </summary>
public class ConsoleUpgradeLog : IUpgradeLog
{
    /// <summary>
    public void LogTrace(string format, params object[] args)
        => Log(Constants.TraceLevel, ConsoleColor.Gray, format, args);

    /// <inheritdoc/>
    public void LogDebug(string format, params object[] args)
        => Log(Constants.DebugLevel, ConsoleColor.Magenta, format, args);

    /// <inheritdoc/>
    public void LogInformation(string format, params object[] args)
        => Log(Constants.InfoLevel, ConsoleColor.White, format, args);

    /// <inheritdoc/>
    public void LogWarning(string format, params object[] args)
        => Log(Constants.WarnLevel, ConsoleColor.Yellow, format, args);

    /// <inheritdoc/>
    public void LogError(string format, params object[] args)
        => Log(Constants.ErrorLevel, ConsoleColor.Red, format, args);

    /// <inheritdoc/>
    public void LogError(Exception ex, string format, params object[] args)
        => Log(Constants.ErrorLevel, ConsoleColor.Red, format, args, ex);

    static void Log(string level, ConsoleColor color, string format, object[] args, Exception ex = null)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        try
        {
            Console.WriteLine($"{DateTimeOffset.Now.ToString(Constants.TimestampFormat)} [{level}] {string.Format(format, args)}");
            if (ex != null)
            {
                Console.WriteLine(ExceptionFormatter.Format(ex));
            }
        }
        finally
        {
            Console.ForegroundColor = oldColor;
        }
    }
}
