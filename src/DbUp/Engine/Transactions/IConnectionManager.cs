using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// Allows you to run an operation with a managed connection
    /// </summary>
    public interface IConnectionManager : IDisposable
    {
        /// <summary>
        /// Tells the connection manager is starting
        /// </summary>
        void UpgradeStarting(IUpgradeLog upgradeLog);

        /// <summary>
        /// Execute a lambda with the connection managed by the connection manager (i.e transactions, reusing connections etc)
        /// </summary>
        /// <param name="action">Action to execute</param>
        void ExecuteCommandsWithManagedConnection(Action<Func<IDbCommand>> action);

        /// <summary>
        /// Execute a lambda with the connection managed by the connection manager (i.e transactions, reusing connections etc)
        /// </summary>
        /// <param name="actionWithResult">Action to execute</param>
        T ExecuteCommandsWithManagedConnection<T>(Func<Func<IDbCommand>, T> actionWithResult);

        /// <summary>
        /// Specifies the transaction strategy
        /// </summary>
        TransactionMode TransactionMode { get; set; }
    }
}