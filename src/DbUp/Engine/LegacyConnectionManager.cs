using System;
using System.Data;

namespace DbUp.Engine
{
    /// <summary>
    /// Allows backwards compatibility with previous API/behaviour of using connection factories with DbUp
    /// </summary>
    public class LegacyConnectionManager : IConnectionManager
    {
        private readonly Func<IDbConnection> connectionFactory;

        /// <summary>
        /// Ctor for LegacyConnectionManager
        /// </summary>
        /// <param name="connectionFactory">The connectionFactory</param>
        public LegacyConnectionManager(Func<IDbConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public void RunWithManagedConnection(Action<IDbConnection> action)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                action(connection);
            }
        }

        public T RunWithManagedConnection<T>(Func<IDbConnection, T> actionWithResult)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return actionWithResult(connection);
            }
        }
    }
}