namespace DbUp.Engine.Output
{
    /// <summary>
    /// Implemented by objects which record the internal details of the database migration.
    /// </summary>
    public interface IUpgradeLog
    {
        /// <summary>
        /// Writes an informational message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void WriteInformation(string format, params object[] args);

        /// <summary>
        /// Writes an error message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void WriteError(string format, params object[] args);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void WriteWarning(string format, params object[] args);
    }
}
