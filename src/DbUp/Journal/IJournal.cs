using DbUp.ScriptProviders;

namespace DbUp.Journal
{
    /// <summary>
    /// This interface is provided to allow different projects to store version information differently.
    /// </summary>
    public interface IJournal
    {
        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        string[] GetExecutedScripts(ILog log);

        /// <summary>
        /// Records an upgrade script for a database.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="log">The log.</param>
        void StoreExecutedScript(SqlScript script, ILog log);
    }
}