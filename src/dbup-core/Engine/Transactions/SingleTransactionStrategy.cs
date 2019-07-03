using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    internal class SingleTransactionStrategy : ITransactionStrategy
    {
        private IDbConnection connection;
        private IDbTransaction transaction;
        private bool errorOccured;
        private IUpgradeLog log;


        public void Execute(Action<Func<IDbCommand>> action)
        {
            if (errorOccured)
                throw new InvalidOperationException("Error occured on previous script execution");

            try
            {
                action(() =>
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    return command;
                });
            }
            catch (Exception)
            {
                errorOccured = true;
                throw;
            }
        }

        public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            if (errorOccured)
                throw new InvalidOperationException("Error occured on previous script execution");

            try
            {
                return actionWithResult(() =>
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    return command;
                });
            }
            catch (Exception)
            {
                errorOccured = true;
                throw;
            }
        }

        public void Initialise(IDbConnection dbConnection, IUpgradeLog upgradeLog)
        {
         
            connection = dbConnection;
            log = upgradeLog;
            upgradeLog.WriteInformation("Beginning transaction");
            transaction = connection.BeginTransaction();
        }

        public void Dispose()
        {
            if (!errorOccured)
                transaction.Commit();
            else
            {
                log.WriteWarning("Error occured when executing scripts, transaction will be rolled back");
                //Restore the executed scripts collection
          
            }

            transaction.Dispose();
        }
    }
}
