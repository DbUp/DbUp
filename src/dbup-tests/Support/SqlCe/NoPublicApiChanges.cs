#if !NETCORE
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.SqlCe;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(SqlCeExtensions).Assembly)
    {
    }
}
#endif
