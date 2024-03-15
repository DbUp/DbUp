using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions;

class TransactionPerScriptStrategy : ITransactionStrategy
{
    IDbConnection connection;
    int? commandTimeout;

    public void Execute(Action<Func<IDbCommand>> action)
    {
        using (var transaction = connection.BeginTransaction())
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
            transaction.Commit();
        }
    }

    public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
    {
        using (var transaction = connection.BeginTransaction())
        {
            var result = actionWithResult(() =>
            {
                var command = connection.CreateCommand();
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
                }

                command.Transaction = transaction;
                return command;
            });
            transaction.Commit();
            return result;
        }
    }

    public void Initialise(
        IDbConnection dbConnection,
        IUpgradeLog upgradeLog,
        List<SqlScript> executedScripts,
        int? executionTimeoutSeconds
    )
    {
        connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        commandTimeout = executionTimeoutSeconds;
    }

    public void Dispose() { }
}
