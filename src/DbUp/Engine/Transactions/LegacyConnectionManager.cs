using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
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

        public void UpgradeStarting(IUpgradeLog upgradeLog)
        {
        }

        public void ExecuteCommandsWithManagedConnection(Action<Func<IDbCommand>> action)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                action(()=>connection.CreateCommand());
            }
        }

        public T ExecuteCommandsWithManagedConnection<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return actionWithResult(()=>connection.CreateCommand());
            }
        }

        public TransactionMode TransactionMode { get; set; }

        public void Dispose()
        {
            
        }
    }
}