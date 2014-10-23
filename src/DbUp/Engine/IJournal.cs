namespace DbUp.Engine
{
    /// <summary>
    /// This interface is provided to allow different projects to store version information differently.
    /// </summary>
    public interface IJournal
    {
        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns></returns>
        string[] GetExecutedScripts();

        /// <summary>
        /// Check validation of script for this database.
        /// </summary>
        /// <param name="script">Executed script.</param>
        /// <returns>True if executed script is valid.</returns>
        bool ValidateScript(SqlScript script);

        /// <summary>
        /// Records an upgrade script for a database.
        /// </summary>
        /// <param name="script">The script.</param>
        void StoreExecutedScript(SqlScript script);
    }
}