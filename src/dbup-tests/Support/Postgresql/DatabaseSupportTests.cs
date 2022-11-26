#if !NETCORE
using DbUp.Builder;
using DbUp.Postgresql;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Postgresql;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.PostgresqlDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalTo(
            (connectionManagerFactory, logFactory)
                => new PostgresqlTableJournal(connectionManagerFactory, logFactory, schema, tableName)
        );
}
#endif
