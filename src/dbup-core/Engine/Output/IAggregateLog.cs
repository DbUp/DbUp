namespace DbUp.Engine.Output
{
    public interface IAggregateLog : IUpgradeLog
    {
        /// <summary>
        /// The number of loggers in the aggregate logger.
        /// </summary>
        int LoggerCount { get; }

        /// <summary>
        /// Identifies if the aggregate logger has any loggers.
        /// </summary>
        bool HasLoggers { get; }

        /// <summary>
        /// Adds the <paramref name="logger"/> to the aggregate logger.
        /// </summary>
        /// <param name="logger"></param>
        void AddLogger(IUpgradeLog logger);
    }
}
