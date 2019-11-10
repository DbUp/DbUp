using System.Data;
using DbUp.Engine.Output;

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
        /// <param name="upgradeLog"></param>
        /// <param name="databaseConnectionManager"></param>
        /// <returns>Created connection</returns>
        IDbConnection CreateConnection(IUpgradeLog upgradeLog, DatabaseConnectionManager databaseConnectionManager);
    }
}