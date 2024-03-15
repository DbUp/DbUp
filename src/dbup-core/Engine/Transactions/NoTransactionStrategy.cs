using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions;

class NoTransactionStrategy : ITransactionStrategy
{
    IDbConnection connection;
    int? commandTimeout;

    public void Execute(Action<Func<IDbCommand>> action)
    {
        action(() =>
        {
            var command = connection.CreateCommand();
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            return command;
        });
    }

    public T Execute<T>(Func<Func<IDbCommand>, T> actionWithResult)
    {
        return actionWithResult(() =>
        {
            var command = connection.CreateCommand();
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            return command;
        });
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
