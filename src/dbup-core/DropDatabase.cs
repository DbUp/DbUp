namespace DbUp
{
    /// <summary>
    /// A fluent interface for dropping the target database.
    /// </summary>
    public static class DropDatabase
    {
        /// <summary>
        ///     Returns the databases supported by DbUp's DropDatabase feature.
        /// </summary>
        public static SupportedDatabasesForDropDatabase For { get; } = new SupportedDatabasesForDropDatabase();
    }
}