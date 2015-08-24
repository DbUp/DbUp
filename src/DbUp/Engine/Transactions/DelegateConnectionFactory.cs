using System;
using System.Data;
using DbUp.Engine.Output;

namespace DbUp.Engine.Transactions
{
    public class DelegateConnectionFactory : IConnectionFactory
    {
        private readonly Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection> createConnection;

        public DelegateConnectionFactory(Func<IUpgradeLog, IDbConnection> createConnection)
            : this((l, _) => createConnection(l))
        {
        }

        public DelegateConnectionFactory(Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection> createConnection)
        {
            this.createConnection = createConnection;
        }
        
        public IDbConnection CreateConnection(IUpgradeLog upgradeLog, DatabaseConnectionManager databaseConnectionManager)
        {
            return createConnection(upgradeLog, databaseConnectionManager);
        }
    }
}