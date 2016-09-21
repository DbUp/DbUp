using System;

namespace DbUp
{
    /// <summary>
    /// A fluent interface for dropping the target database.
    /// </summary>
    public static class DropDatabase
    {
        private static readonly SupportedDatabasesForDropDatabase Instance = new SupportedDatabasesForDropDatabase();

        /// <summary>
        /// Returns the databases supported by DbUp's DropDatabase feature.
        /// </summary>
        public static SupportedDatabasesForDropDatabase For
        {
            get { return Instance; }
        }
    }
}