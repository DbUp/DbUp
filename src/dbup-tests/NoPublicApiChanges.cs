using DbUp.Tests.Common;

namespace DbUp.Tests;

public class NoPublicApiChanges : NoPublicApiChangesBase
{
    public NoPublicApiChanges()
        : base(typeof(UpgradeEngine).Assembly)
    {
    }
}
