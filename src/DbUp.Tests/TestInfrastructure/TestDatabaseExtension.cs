using System;
using System.Data;
using DbUp.Builder;
using DbUp.Engine.Transactions;

namespace DbUp.Tests.TestInfrastructure
{
    /// <summary>
    /// Configures DbUp to use SqlServer with a fake connection
    /// </summary>
    public static class TestDatabaseExtension
    {
        public static UpgradeEngineBuilder TestDatabase(this SupportedDatabases supportedDatabases, IDbConnection connection)
        {
            var builder = supportedDatabases.SqlDatabase("");
            builder.Configure(c => ((DatabaseConnectionManager)c.ConnectionManager).OverrideFactoryForTest(new DelegateConnectionFactory(l => connection)));
            return builder;
        }
    }
}