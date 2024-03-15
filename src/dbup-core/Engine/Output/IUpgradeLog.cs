using System;

namespace DbUp.Engine.Output;

/// <summary>
/// Implemented by objects which record the internal details of the database migration.
/// </summary>
public interface IUpgradeLog
{
    /// <summary>
    /// Writes an informational message to the log.
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void LogTrace(string format, params object[] args);

    /// <summary>
    /// Writes a debug message to the log.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void LogDebug(string format, params object[] args);

    /// <summary>
    /// Writes an informational message to the log.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void LogInformation(string format, params object[] args);

    /// <summary>
    /// Writes a warning message to the log.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void LogWarning(string format, params object[] args);

    /// Writes an error message to the log.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void LogError(string format, params object[] args);

    /// <summary>
    /// Writes a warning message to the log.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void LogError(Exception ex, string format, params object[] args);
}
