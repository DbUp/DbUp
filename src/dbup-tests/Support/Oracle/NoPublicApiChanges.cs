using DbUp.Oracle;
using DbUp.Tests.Common;

namespace DbUp.Tests.Providers.Oracle;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(OracleExtensions).Assembly)
    {
    }
}
