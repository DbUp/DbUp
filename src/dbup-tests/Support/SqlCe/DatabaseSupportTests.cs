#if !NETCORE
using DbUp.Builder;
using DbUp.SqlCe;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SqlCe;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.SqlCeDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalTo(
            (connectionManagerFactory, logFactory)
                => new SqlCeTableJournal(connectionManagerFactory, logFactory, schema, tableName)
        );
}
#endif
