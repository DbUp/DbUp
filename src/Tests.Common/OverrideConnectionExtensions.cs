using System;
using System.Data;
using DbUp.Builder;
using DbUp.Engine.Transactions;

namespace DbUp.Tests.Common
{
    public static class OverrideConnectionExtensions
    {
        public static UpgradeEngineBuilder OverrideConnectionFactory(this UpgradeEngineBuilder engineBuilder, IDbConnection connection)
        {
            return engineBuilder.OverrideConnectionFactory(new DelegateConnectionFactory(l => connection));
        }

        public static UpgradeEngineBuilder OverrideConnectionFactory(this UpgradeEngineBuilder engineBuilder, IConnectionFactory connectionFactory)
        {
            engineBuilder.Configure(c => ((DatabaseConnectionManager)c.ConnectionManager).OverrideFactoryForTest(connectionFactory));
            return engineBuilder;
        }

    }
}
