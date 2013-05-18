using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    class TransactionPerScriptStrategy : ITransactionStrategy
    {
        private readonly Func<IDbConnection> connectionFactory;

        public TransactionPerScriptStrategy(Func<IDbConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public void Execute(Action<Func<IDbCommand>> action)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    action(() =>
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        return command;
                    });
                    transaction.Commit();
                }
            }
        }

        public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var result = actionWithResult(() =>
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        return command;
                    });
                    transaction.Commit();
                    return result;
                }
            }
        }

        public void Initialise(IUpgradeLog upgradeLog) { }

        public void Dispose() { }
    }
}