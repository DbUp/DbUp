using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    class SingleTransactionAlwaysRollbackStrategy : ITransactionStrategy
    {
        IDbConnection connection;
        IDbTransaction transaction;
        bool errorOccured;
        IUpgradeLog log;
        SqlScript[] executedScriptsListBeforeExecution;
        List<SqlScript> executedScriptsCollection;

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

        public void Initialise(IDbConnection dbConnection, IUpgradeLog upgradeLog, List<SqlScript> executedScripts)
        {
            executedScriptsCollection = executedScripts;
            executedScriptsListBeforeExecution = executedScripts.ToArray();
            connection = dbConnection;
            log = upgradeLog;
            upgradeLog.WriteInformation("Beginning transaction");
            transaction = connection.BeginTransaction();
        }

        public void Dispose()
        {
            if (!errorOccured)
            {
                log.WriteInformation("Success! No errors have occured when executing scripts, transaction will be rolled back");
            }
            else
            {
                log.WriteWarning("Error occured when executing scripts, transaction will be rolled back");
            }

            // Always rollback
            transaction?.Rollback();

            //Restore the executed scripts collection
            executedScriptsCollection.Clear();
            executedScriptsCollection.AddRange(executedScriptsListBeforeExecution);

            transaction?.Dispose();
        }
    }
}
