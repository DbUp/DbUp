using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    class NoTransactionStrategy : ITransactionStrategy
    {
        IDbConnection connection;

        public void Execute(Action<Func<IDbCommand>> action)
        {
            action(() => connection.CreateCommand());
        }

        public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            return actionWithResult(() => connection.CreateCommand());
        }

        public void Initialise(IDbConnection dbConnection, IUpgradeLog upgradeLog, List<SqlScript> executedScripts)
        {
            connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        public void Dispose() { }
    }
}
