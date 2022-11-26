#if !NETCORE
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SqlAnywhere;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(SqlAnywhereExtensions).Assembly)
    {
    }
}
#endif
