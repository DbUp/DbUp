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
        private IDbConnection upgradeConnection;
	    protected Func<IUpgradeLog> log;

        /// <summary>
        /// Manages Database Connections
        /// </summary>
        protected DatabaseConnectionManager()
        {
            transactionStrategyFactory = new Dictionary<TransactionMode, Func<ITransactionStrategy>>
            {
                {TransactionMode.NoTransaction, ()=>new NoTransactionStrategy()},
                {TransactionMode.SingleTransaction, ()=>new SingleTrasactionStrategy()},
                {TransactionMode.TransactionPerScript, ()=>new TransactionPerScriptStrategy()}
            };
        }

        /// <summary>
        /// Creates a database connection for the current database engine
        /// </summary>
        protected abstract IDbConnection CreateConnection();

        /// <summary>
        /// Tells the connection manager is starting
        /// </summary>
        public IDisposable OperationStarting(IUpgradeLog upgradeLog)
        {
            upgradeConnection = CreateConnection();
            if (upgradeConnection.State == ConnectionState.Closed)
                upgradeConnection.Open();
            if (transactionStrategy != null)
                throw new InvalidOperationException("OperationStarting is meant to be called by DbUp and can only be called once");
            transactionStrategy = transactionStrategyFactory[TransactionMode]();
            transactionStrategy.Initialise(upgradeConnection, upgradeLog);

            return new DelegateDisposable(() =>
            {
                transactionStrategy.Dispose();
                upgradeConnection.Dispose();
                transactionStrategy = null;
                upgradeConnection = null;
            });
        }

        /// <summary>
        /// Executes an action using the specfied transaction mode 
        /// </summary>
        /// <param name="action">The action to execute</param>
        public void ExecuteCommandsWithManagedConnection(Action<Func<IDbCommand>> action)
        {
            transactionStrategy.Execute(action);
        }

        /// <summary>
        /// Executes an action which has a result using the specfied transaction mode 
        /// </summary>
        /// <param name="actionWithResult">The action to execute</param>
        /// <typeparam name="T">The result type</typeparam>
        /// <returns>The result of the command</returns>
        public T ExecuteCommandsWithManagedConnection<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            return transactionStrategy.Execute(actionWithResult);
        }

        /// <summary>
        /// The transaction strategy that DbUp should use
        /// </summary>
        public TransactionMode TransactionMode { get; set; }

		/// <summary>
		/// The logging function that will be used to log the print statements etc to
		/// </summary>
		/// <param name="log"></param>
		public void InjectLog(Func<IUpgradeLog> log)
		{
			this.log = log;
		}

	    /// <summary>
        /// Splits a script into commands, for example SQL Server separates command by the GO statement
        /// </summary>
        /// <param name="scriptContents">The script</param>
        /// <returns>A list of SQL Commands</returns>
        public abstract IEnumerable<string> SplitScriptIntoCommands(string scriptContents);
    }
}