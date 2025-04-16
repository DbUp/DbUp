using System.Reflection;
using System.Runtime.CompilerServices;
using Assent;
using Assent.Namers;
using PublicApiGenerator;
using Xunit;

namespace DbUp.Tests.Common;

public abstract class NoPublicApiChangesBase
{
    private readonly Assembly assembly;
    readonly string? callerFilePath;

    public NoPublicApiChangesBase(Assembly assembly, [CallerFilePath] string? callerFilePath = null)
    {
        this.assembly = assembly;
        this.callerFilePath = callerFilePath;
    }

    [Fact]
    public void Run()
    {
        var options = new ApiGeneratorOptions
        {
            OrderBy = OrderMode.NamespaceThenFullName,
            ExcludeAttributes = ["System.Diagnostics.DebuggerDisplayAttribute"],
        };
        var result = assembly.GeneratePublicApi(options);

        var config = new Configuration()
            .UsingExtension("cs")
            .UsingNamer(new SubdirectoryNamer("ApprovalFiles"));

        // Automatically approve the change, make sure to check the result before committing
        // config = config.UsingReporter((received, approved) => System.IO.File.Copy(received, approved, true));

        this.Assent(result, config,  filePath: callerFilePath);
    }
}
