using DbUp.ScriptProviders;

namespace DbUp.Journal
{
    /// <summary>
    /// This interface is provided to allow different projects to store version information differently.
    /// </summary>
    public interface IJournal
    {
        /// <summary>
        /// Recalls the version number of a database specified in a given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        string[] GetExecutedScripts(string connectionString, ILog log);

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="script">The script.</param>
        /// <param name="log">The log.</param>
        void StoreExecutedScript(string connectionString, SqlScript script, ILog log);
    }
}