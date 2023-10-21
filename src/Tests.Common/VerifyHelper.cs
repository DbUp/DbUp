using VerifyTests;

namespace DbUp.Tests.Common;

public static class VerifyHelper
{
    public static VerifySettings GetVerifySettings(bool uniqueForFramework = false)
    {
        VerifySettings settings = new();
        settings.UseDirectory("ApprovalFiles");
        if (uniqueForFramework)
            settings.UniqueForTargetFramework();
        settings.ScrubLinesWithReplace(Scrubbers.ScrubDates);
        return settings;
    }
}
