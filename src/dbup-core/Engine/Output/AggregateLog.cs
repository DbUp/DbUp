using System;
using System.Collections.Generic;
using System.Linq;

namespace DbUp.Engine.Output
{
    public class AggregateLog : IAggregateLog
    {
        public AggregateLog(IEnumerable<IUpgradeLog> loggers = null)
        {
            this._loggers = (loggers ?? Enumerable.Empty<IUpgradeLog>()).ToList();
        }

        private readonly List<IUpgradeLog> _loggers;

        /// <inheritdoc/>
        public int LoggerCount => _loggers.Count;

        /// <inheritdoc/>
        public bool HasLoggers => this.LoggerCount > 0;

        /// <inheritdoc/>
        public void AddLogger(IUpgradeLog logger)
            => _loggers.Add(logger ?? throw new ArgumentException(nameof(logger)));

        /// <inheritdoc/>
        public void LogTrace(string format, params object[] args)
            => Log(_loggers, x => x.LogTrace(format, args));

        /// <inheritdoc/>
        public void LogDebug(string format, params object[] args)
            => Log(_loggers, x => x.LogDebug(format, args));

        /// <inheritdoc/>
        public void LogInformation(string format, params object[] args)
            => Log(_loggers, x => x.LogInformation(format, args));

        /// <inheritdoc/>
        public void LogWarning(string format, params object[] args)
            => Log(_loggers, x => x.LogWarning(format, args));

        /// <inheritdoc/>
        public void LogError(string format, params object[] args)
            => Log(_loggers, x => x.LogError(format, args));

        /// <inheritdoc/>
        public void LogError(Exception ex, string format, params object[] args)
            => Log(_loggers, x => x.LogError(ex, format, args));

        /// <summary>
        /// Logs the message to all loggers.
        /// </summary>
        /// <typeparam name="IUpgradeLog"></typeparam>
        /// <param name="loggers"></param>
        /// <param name="writeTo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        static void Log<IUpgradeLog>(IEnumerable<IUpgradeLog> loggers, Action<IUpgradeLog> writeTo)
        {
            if (writeTo is null)
            {
                throw new ArgumentNullException(nameof(writeTo));
            }

            if (loggers?.Any() != true)
            {
                return;
            }

            foreach (var log in loggers)
            {
                writeTo(log);
            }
        }
    }
}
