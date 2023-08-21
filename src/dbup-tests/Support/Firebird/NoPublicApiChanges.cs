#if !NETCORE
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Firebird;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(FirebirdExtensions).Assembly)
    {
    }
}
#endif
