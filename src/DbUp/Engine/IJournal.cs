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
        /// Records an upgrade script for a database.
        /// </summary>
        /// <param name="script">The script.</param>
        void StoreExecutedScript(SqlScript script);

        /// <summary>
        /// Removes the record for the rolled back executed script in the database.
        /// </summary>
        /// <param name="script">The executed script that got rolled back.</param>
        void RemoveExecutedScript(SqlScript script);
    }
}