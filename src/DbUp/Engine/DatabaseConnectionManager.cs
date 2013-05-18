using System;
using System.Data;

namespace DbUp.Engine
{
    /// <summary>
    /// Manages Sql Database Connections
    /// </summary>
    public abstract class DatabaseConnectionManager : IConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        protected DatabaseConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected abstract IDbConnection CreateConnection(string connectionString);

        public void RunWithManagedConnection(Action<IDbConnection> action)
        {
            using (var connection = CreateConnection(connectionString))
            {
                connection.Open();

                action(connection);
            }
        }

        public T RunWithManagedConnection<T>(Func<IDbConnection, T> actionWithResult)
        {
            using (var connection = CreateConnection(connectionString))
            {
                connection.Open();

                return actionWithResult(connection);
            }
        }
    }
}