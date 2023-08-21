#if !NETCORE
using DbUp.Builder;
using DbUp.MySql;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.MySql;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.MySqlDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalTo(
            (connectionManagerFactory, logFactory)
                => new MySqlTableJournal(connectionManagerFactory, logFactory, schema, tableName)
        );
}
#endif
