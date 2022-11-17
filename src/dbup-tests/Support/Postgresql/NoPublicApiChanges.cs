#if !NETCORE
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Postgresql;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(PostgresqlExtensions).Assembly)
    {
    }
}
#endif
