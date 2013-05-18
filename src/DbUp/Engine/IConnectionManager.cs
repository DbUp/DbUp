using System;
using System.Data;

namespace DbUp.Engine
{
    /// <summary>
    /// Allows you to run an operation with a managed connection
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Execute a lambda with the connection managed by the connection manager (i.e transactions, reusing connections etc)
        /// </summary>
        /// <param name="action">Action to execute</param>
        void RunWithManagedConnection(Action<IDbConnection> action);

        /// <summary>
        /// Execute a lambda with the connection managed by the connection manager (i.e transactions, reusing connections etc)
        /// </summary>
        /// <param name="actionWithResult">Action to execute</param>
        T RunWithManagedConnection<T>(Func<IDbConnection, T> actionWithResult);
    }
}