#if !NETCORE
using DbUp.Builder;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Redshift;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.RedshiftDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalToRedshiftTable(schema, tableName);
}
#endif
