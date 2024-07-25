using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DbUp.Engine.Output
{
    /// <summary>
    /// Writes log entries to a Microsoft.Extensions.Logging.ILogger.
    /// </summary>
    public class MicrosoftUpgradeLog : IUpgradeLog
    {
        public MicrosoftUpgradeLog([NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<UpgradeEngine>();
        }

        public MicrosoftUpgradeLog([NotNull] ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        readonly ILogger _logger;

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
