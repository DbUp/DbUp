using System;
using System.Data;
using System.Data.SqlServerCe;
using DbUp.Engine;

namespace DbUp.SqlCe
{
    /// <summary>
    /// Manages SqlCe Database Connections
    /// </summary>
    public class SqlCeConnectionManager : IConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages SqlCe Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlCeConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void RunWithManagedConnection(Action<IDbConnection> action)
        {
            using (var connection = new SqlCeConnection(connectionString))
            {
                connection.Open();

                action(connection);
            }
        }

        public T RunWithManagedConnection<T>(Func<IDbConnection, T> actionWithResult)
        {
            using (var connection = new SqlCeConnection(connectionString))
            {
                connection.Open();

                return actionWithResult(connection);
            }
        }
    }
}