using DbUp.Builder;
using DbUp.SQLite;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SQLite;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.SQLiteDatabase("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalTo(
            (connectionManagerFactory, logFactory)
                => new SQLiteTableJournal(connectionManagerFactory, logFactory, tableName)
        );
}
