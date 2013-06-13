using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    class NoTransactionStrategy : ITransactionStrategy
    {
        private readonly Func<IDbConnection> connectionFactory;

        public NoTransactionStrategy(Func<IDbConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public void Execute(Action<Func<IDbCommand>> action)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                action(()=>connection.CreateCommand());
            }
        }

        public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return actionWithResult(() => connection.CreateCommand());
            }
        }

        public void Initialise(IUpgradeLog upgradeLog)
        {}

        public void Dispose() { }
    }
}