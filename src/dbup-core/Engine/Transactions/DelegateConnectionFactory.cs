using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    public class DelegateConnectionFactory : IConnectionFactory
    {
        readonly Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection> createConnection;

        public DelegateConnectionFactory(Func<IUpgradeLog, IDbConnection> createConnection)
            : this((l, _) => createConnection(l))
        {
        }

        public DelegateConnectionFactory(Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection> createConnection)
        {
            this.createConnection = createConnection ?? throw new ArgumentNullException(nameof(createConnection));
        }

        public IDbConnection CreateConnection(IUpgradeLog upgradeLog, DatabaseConnectionManager databaseConnectionManager)
        {
            return createConnection(upgradeLog, databaseConnectionManager);
        }
    }
}
