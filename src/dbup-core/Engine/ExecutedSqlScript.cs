namespace DbUp.Engine
{
    /// <summary>
    /// Represents a SQL Server script that comes from the journal database table
    /// </summary>
    public class ExecutedSqlScript
    {
        /// <summary>
        /// Name of the script
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Hash value of the contents of the file
        /// </summary>
        public string Hash { get; set; }
    }
}
