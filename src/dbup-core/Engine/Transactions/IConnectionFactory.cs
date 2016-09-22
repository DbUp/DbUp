using System.Data;

namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// Responsible for creating a database connection to the database
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Creates a database connection, the connection can be opened or closed
        /// </summary>
        /// <param name="databaseConnectionManager"></param>
        /// <returns></returns>
        IDbConnection CreateConnection(DatabaseConnectionManager databaseConnectionManager);
    }
}