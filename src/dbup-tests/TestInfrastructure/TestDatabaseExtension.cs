using System.Data;
using DbUp.Builder;
using DbUp.Tests.Common;

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
            builder.OverrideConnectionFactory(connection);
            return builder;
        }
    }
}
