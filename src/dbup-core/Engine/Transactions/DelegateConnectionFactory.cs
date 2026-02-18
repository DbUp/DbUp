using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions;

/// <summary>
/// A connection factory that uses a delegate to create connections.
/// </summary>
public class DelegateConnectionFactory : IConnectionFactory
{
    readonly Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection> createConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateConnectionFactory"/> class.
    /// </summary>
    /// <param name="createConnection">The delegate to create a connection.</param>
    public DelegateConnectionFactory(Func<IUpgradeLog, IDbConnection> createConnection)
        : this((l, _) => createConnection(l))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateConnectionFactory"/> class.
    /// </summary>
    /// <param name="createConnection">The delegate to create a connection.</param>
    public DelegateConnectionFactory(Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection> createConnection)
    {
        this.createConnection = createConnection ?? throw new ArgumentNullException(nameof(createConnection));
    }

    /// <inheritdoc/>
    public IDbConnection CreateConnection(IUpgradeLog upgradeLog, DatabaseConnectionManager databaseConnectionManager)
    {
        return createConnection(upgradeLog, databaseConnectionManager);
    }
}
