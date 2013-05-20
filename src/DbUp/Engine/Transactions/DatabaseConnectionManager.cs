using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// Manages Sql Database Connections
    /// </summary>
    public abstract class DatabaseConnectionManager : IConnectionManager
    {
        private ITransactionStrategy transactionStrategy;
        private readonly Dictionary<TransactionMode, Func<ITransactionStrategy>> transactionStrategyFactory;

        /// <summary>
        /// Manages Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        protected DatabaseConnectionManager(string connectionString)
        {
            transactionStrategyFactory = new Dictionary<TransactionMode, Func<ITransactionStrategy>>
            {
                {TransactionMode.NoTransaction, ()=>new NoTransactionStrategy(()=>CreateConnection(connectionString))},
                {TransactionMode.SingleTransaction, ()=>new SingleTrasactionStrategy(()=>CreateConnection(connectionString))},
                {TransactionMode.TransactionPerScript, ()=>new TransactionPerScriptStrategy(()=>CreateConnection(connectionString))}
            };
        }

        protected abstract IDbConnection CreateConnection(string connectionString);

        public void UpgradeStarting(IUpgradeLog upgradeLog)
        {
            if (transactionStrategy != null)
                throw new InvalidOperationException("UpgradeStarting is meant to be called by DbUp and can only be called once");
            transactionStrategy = transactionStrategyFactory[TransactionMode]();
            transactionStrategy.Initialise(upgradeLog);
        }

        public void ExecuteCommandsWithManagedConnection(Action<Func<IDbCommand>> action)
        {
            transactionStrategy.Execute(action);
        }

        public T ExecuteCommandsWithManagedConnection<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            return transactionStrategy.Execute(actionWithResult);
        }

        public TransactionMode TransactionMode { get; set; }

        public void Dispose()
        {
            transactionStrategy.Dispose();
        }
    }
}