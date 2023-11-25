using DbUp.Builder;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SqlServer;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.SqlDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalToSqlTable(schema, tableName);
}
