using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions;

class SingleTransactionStrategy : ITransactionStrategy
{
    IDbConnection connection;
    int? commandTimeout;
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
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
                }

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
            throw new InvalidOperationException("Error occurred on previous script execution");

        try
        {
            return actionWithResult(() =>
            {
                var command = connection.CreateCommand();
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
                }

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

    public void Initialise(
        IDbConnection dbConnection,
        IUpgradeLog upgradeLog,
        List<SqlScript> executedScripts,
        int? executionTimeoutSeconds
    )
    {
        executedScriptsCollection = executedScripts;
        executedScriptsListBeforeExecution = executedScripts.ToArray();
        connection = dbConnection;
        commandTimeout = executionTimeoutSeconds;
        log = upgradeLog;
        upgradeLog.LogInformation("Beginning transaction");
        transaction = connection.BeginTransaction();
    }

    public void Dispose()
    {
        if (!errorOccured)
        {
            transaction.Commit();
        }
        else
        {
            log.LogWarning("Error occured when executing scripts, transaction will be rolled back");
            //Restore the executed scripts collection
            executedScriptsCollection.Clear();
            executedScriptsCollection.AddRange(executedScriptsListBeforeExecution);
        }

        transaction.Dispose();
    }
}
