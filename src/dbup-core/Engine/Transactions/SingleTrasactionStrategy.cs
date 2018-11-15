using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    internal class SingleTrasactionStrategy : ITransactionStrategy
    {
        private IDbConnection connection;
        private IDbTransaction transaction;
        private bool errorOccurred;
        private IUpgradeLog log;
        private SqlScript[] executedScriptsListBeforeExecution;
        private List<SqlScript> executedScriptsCollection;

        public void Execute(Action<Func<IDbCommand>> action)
        {
            if (errorOccurred)
                throw new InvalidOperationException("Error occurred on previous script execution");

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
                errorOccurred = true;
                throw;
            }
        }

        public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            if (errorOccurred)
                throw new InvalidOperationException("Error occurred on previous script execution");

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
                errorOccurred = true;
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
            if (!errorOccurred)
                transaction.Commit();
            else
            {
                log.WriteWarning("Error occurred when executing scripts, transaction will be rolled back");
                //Restore the executed scripts collection
                executedScriptsCollection.Clear();
                executedScriptsCollection.AddRange(executedScriptsListBeforeExecution);
            }

            transaction.Dispose();
        }
    }
}