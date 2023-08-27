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
        public void LogTrace(string format, params object[] args) =>
            _logger?.LogTrace(format, args);

        /// <inheritdoc/>
        public void LogDebug(string format, params object[] args) =>
            _logger?.LogDebug(format, args);

        /// <inheritdoc/>
        public void LogInformation(string format, params object[] args) =>
            _logger?.LogInformation(format, args);

        /// <inheritdoc/>
        public void LogWarning(string format, params object[] args) =>
            _logger?.LogWarning(format, args);

        /// <inheritdoc/>
        public void LogError(string format, params object[] args) =>
            _logger?.LogError(format, args);

        /// <inheritdoc/>
        public void LogError(Exception ex, string format, params object[] args) =>
            _logger?.LogError(ex, format, args);
    }
}
