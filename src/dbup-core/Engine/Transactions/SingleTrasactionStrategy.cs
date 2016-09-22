using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Logging;

namespace DbUp.Engine.Transactions
{
    internal class SingleTrasactionStrategy : ITransactionStrategy
    {
        private static readonly ILog log = LogProvider.For<SingleTrasactionStrategy>();

        private IDbConnection connection;
        private IDbTransaction transaction;
        private bool errorOccured;
        private SqlScript[] executedScriptsListBeforeExecution;
        private List<SqlScript> executedScriptsCollection;

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

        public void Initialise(IDbConnection dbConnection, List<SqlScript> executedScripts)
        {
            executedScriptsCollection = executedScripts;
            executedScriptsListBeforeExecution = executedScripts.ToArray();
            connection = dbConnection;
            log.Info("Beginning transaction");
            transaction = connection.BeginTransaction();
        }

        public void Dispose()
        {
            if (!errorOccured)
                transaction.Commit();
            else
            {
                log.Warn("Error occured when executing scripts, transaction will be rolled back");
                //Restore the executed scripts collection
                executedScriptsCollection.Clear();
                executedScriptsCollection.AddRange(executedScriptsListBeforeExecution);
            }

            transaction.Dispose();
        }
    }
}