using DbUp.Builder;
using DbUp.Oracle;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Oracle;

public class DatabaseSupportTests : DatabaseSupportTestsBase
{
    public DatabaseSupportTests() : base()
    {
    }

    protected override UpgradeEngineBuilder DeployTo(SupportedDatabases to)
        => to.OracleDatabaseWithDefaultDelimiter("");

    protected override UpgradeEngineBuilder AddCustomNamedJournalToBuilder(UpgradeEngineBuilder builder, string schema, string tableName)
        => builder.JournalTo(
            (connectionManagerFactory, logFactory)
                => new OracleTableJournal(connectionManagerFactory, logFactory, schema, tableName)
        );
}
