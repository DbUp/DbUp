using System;
using System.Data;

namespace DbUp.Engine.Transactions
{
    public class DelegateConnectionFactory : IConnectionFactory
    {
        private readonly Func<DatabaseConnectionManager, IDbConnection> createConnection;

        public DelegateConnectionFactory(Func<IDbConnection> createConnection)
            : this(_ => createConnection())
        {
        }

        public DelegateConnectionFactory(Func<DatabaseConnectionManager, IDbConnection> createConnection)
        {
            this.createConnection = createConnection;
        }
        
        public IDbConnection CreateConnection(DatabaseConnectionManager databaseConnectionManager)
        {
            return createConnection(databaseConnectionManager);
        }
    }
}