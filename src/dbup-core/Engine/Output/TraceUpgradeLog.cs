﻿using System;
using System.Diagnostics;

namespace DbUp.Engine.Output;

/// <summary>
/// A log that writes to System.Diagnostics.Trace.
/// </summary>
public class TraceUpgradeLog : IUpgradeLog
{
    /// <summary>
    public void LogTrace(string format, params object[] args)
        => Log(Constants.TraceLevel, format, args);

    /// </summary>
    public void LogDebug(string format, params object[] args)
        => Log(Constants.DebugLevel, format, args);

    /// <inheritdoc/>
    public void LogInformation(string format, params object[] args)
        => Log(Constants.InfoLevel, format, args);

    /// <inheritdoc/>
    public void LogWarning(string format, params object[] args)
        => Log(Constants.WarnLevel, format, args);

    /// <inheritdoc/>
    public void LogError(string format, params object[] args)
        => Log(Constants.ErrorLevel, format, args);

    /// <inheritdoc/>
    public void LogError(Exception ex, string format, params object[] args)
        => Log(Constants.ErrorLevel, format, args, ex);

    static void Log(string level, string format, object[] args, Exception ex = null)
    {
        Trace.WriteLine($"{DateTimeOffset.Now.ToString(Constants.TimestampFormat)} [{level}] {string.Format(format, args)}");
        if (ex != null)
            /// <summary>
            /// Writes an error message to the log.
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="args">The args.</param>
        {
            Trace.WriteLine(ExceptionFormatter.Format(ex));
        }
        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
    }
}
