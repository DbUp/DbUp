#if !NETCORE
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.MySql;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(MySqlExtensions).Assembly)
    {
    }
}
#endif
