namespace DbUp
{
    /// <summary>
    /// A fluent interface for creating the target database.
    /// </summary>
    public static class EnsureDatabase
    {
        private static readonly SupportedDatabasesForEnsureDatabase Instance = new SupportedDatabasesForEnsureDatabase();

        /// <summary>
        /// Returns the databases supported by DbUp's EnsureDatabase feature.
        /// </summary>
        public static SupportedDatabasesForEnsureDatabase For
        {
            get { return Instance; }
        }
    }
}