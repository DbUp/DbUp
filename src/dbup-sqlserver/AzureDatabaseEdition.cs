namespace DbUp.SqlServer
{
    /// <summary>
    /// Azure SQL Insance Edition for Database Creation
    /// </summary>
    public enum AzureDatabaseEdition
    {
        /// <summary>
        /// Not an Azure SQL Insance
        /// </summary>
        None = 0,
        /// <summary>
        /// Basic Azure SQL Insance
        /// </summary>
        Basic,
        /// <summary>
        /// Standard Azure SQL Insance
        /// </summary>
        Standard,
        /// <summary>
        /// Premium Azure SQL Insance
        /// </summary>
        Premium
    }
}
