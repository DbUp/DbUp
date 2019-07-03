using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// The transaction strategy being to be used by the DatabaseConnectionManager
    /// </summary>
    public interface ITransactionStrategy : IDisposable
    {
        /// <summary>
        /// Executes an action
        /// </summary>
        /// <param name="action"></param>
        void Execute(Action<Func<IDbCommand>> action);

        /// <summary>
        /// Executes an action which has a result
        /// </summary>
        /// <param name="actionWithResult"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult);

        /// <summary>
        /// Initializes the transaction strategy with the upgrade log
        /// </summary>
        /// <param name="dbConnection">The database connection.</param>
        /// <param name="upgradeLog">The upgrade log.</param>
        void Initialise(IDbConnection dbConnection, IUpgradeLog upgradeLog);
    }
}