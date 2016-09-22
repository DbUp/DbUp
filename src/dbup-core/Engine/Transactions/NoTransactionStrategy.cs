using System;
using System.Collections.Generic;
using System.Data;

namespace DbUp.Engine.Transactions
{
    internal class NoTransactionStrategy : ITransactionStrategy
    {
        private IDbConnection connection;

        public void Execute(Action<Func<IDbCommand>> action)
        {
            action(()=>connection.CreateCommand());
        }

        public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            return actionWithResult(() => connection.CreateCommand());
        }

        public void Initialise(IDbConnection dbConnection, List<SqlScript> executedScripts)
        {
            connection = dbConnection;
        }

        public void Dispose() { }
    }
}