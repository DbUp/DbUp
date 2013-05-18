using System;
using System.Data;
using System.Data.SqlClient;
using DbUp.Engine;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// Manages Sql Database Connections
    /// </summary>
    public class SqlConnectionManager : IConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages Sql Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void RunWithManagedConnection(Action<IDbConnection> action)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                action(connection);
            }
        }

        public T RunWithManagedConnection<T>(Func<IDbConnection, T> actionWithResult)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return actionWithResult(connection);
            }
        }
    }
}