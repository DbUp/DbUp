using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SQLite;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(SQLiteExtensions).Assembly)
    {
    }
}
