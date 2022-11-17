#if !NETCORE
using DbUp.Builder;
using DbUp.Firebird;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Firebird;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.FirebirdDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalTo(
            (connectionManagerFactory, logFactory)
                => new FirebirdTableJournal(connectionManagerFactory, logFactory, tableName)
        );
}
#endif
