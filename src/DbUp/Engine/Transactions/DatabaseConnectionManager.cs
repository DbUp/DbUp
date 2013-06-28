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
        private readonly Dictionary<TransactionMode, Func<ITransactionStrategy>> transactionStrategyFactory = new Dictionary<TransactionMode, Func<ITransactionStrategy>>();
        private readonly SharedConnection sharedConnection;

        /// <summary>
        /// Manages Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        protected DatabaseConnectionManager(string connectionString)
        {
            Initialize(() => CreateConnection(connectionString));
        }

        /// <summary>
        /// Uses and Manages specified connection for performing database upgrade
        /// </summary>
        /// <param name="connection">database connection object used by DpUp upgrade engine</param>
        protected DatabaseConnectionManager(IDbConnection connection)
        {   
            sharedConnection = new SharedConnection(connection);
            Initialize(() => sharedConnection);
        }

        private void Initialize(Func<IDbConnection> connectionFactory)
        {
            transactionStrategyFactory.Add(TransactionMode.NoTransaction, ()=> new NoTransactionStrategy(connectionFactory));
            transactionStrategyFactory.Add(TransactionMode.SingleTransaction, ()=> new SingleTrasactionStrategy(connectionFactory));
            transactionStrategyFactory.Add(TransactionMode.TransactionPerScript, ()=>new TransactionPerScriptStrategy(connectionFactory));
        }

        /// <summary>
        /// Creates a database connection for the current database engine
        /// </summary>
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

        public abstract IEnumerable<string> SplitScriptIntoCommands(string scriptContents);

        public void Dispose()
        {
            try
            {
                transactionStrategy.Dispose();
            }
            finally
            {
                if (sharedConnection != null)
                    sharedConnection.DoClose();
            }
        }

        private Func<IDbConnection> GetConnectionFactory(string connectionString)
        {
            return () => CreateConnection(connectionString);
        }
    }
}