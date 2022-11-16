#if !NETCORE
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Redshift;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(RedshiftExtensions).Assembly)
    {
    }
}
#endif
