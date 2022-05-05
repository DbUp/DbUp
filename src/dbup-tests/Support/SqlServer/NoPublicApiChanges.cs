using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SqlServer;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(SqlServerExtensions).Assembly, true)
    {
    }
}
