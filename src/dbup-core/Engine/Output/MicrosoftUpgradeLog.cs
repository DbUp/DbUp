using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DbUp.Engine.Output
{
    /// <summary>
    /// Writes log entries to a Microsoft.Extensions.Logging.ILogger.
    /// </summary>
    public class MicrosoftUpgradeLog : IUpgradeLog
    {
        public MicrosoftUpgradeLog(ILoggerFactory loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            _logger = loggerFactory?.CreateLogger<UpgradeEngine>()
                      ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public MicrosoftUpgradeLog(ILogger logger)
        {
            _logger = logger ?? NullLogger.Instance;
        }

        private readonly ILogger _logger;

        /// <summary>
        /// A logger that does nothing.
        /// </summary>
        public static IUpgradeLog DevNull => new Lazy<IUpgradeLog>(() => new MicrosoftUpgradeLog(NullLoggerFactory.Instance)).Value;

        /// <inheritdoc/>
        public void LogTrace(string message, params object[] args) =>
            _logger?.LogTrace(message, args);

        /// <inheritdoc/>
        public void LogDebug(string message, params object[] args) =>
            _logger?.LogDebug(message, args);

        /// <inheritdoc/>
        public void LogInformation(string message, params object[] args) =>
            _logger?.LogInformation(message, args);

        /// <inheritdoc/>
        public void LogWarning(string message, params object[] args) =>
            _logger?.LogWarning(message, args);

        /// <inheritdoc/>
        public void LogError(string message, params object[] args) =>
            _logger?.LogError(message, args);

        /// <inheritdoc/>
        public void LogError(Exception ex, string message, params object[] args) =>
            _logger?.LogError(ex, message, args);
    }
}
