using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DbUp.Tests.Engine.Output
{
    public class MicrosoftLoggingTests : BaseLoggingTest
    {
        protected override IUpgradeLog CreateLogger() => new MicrosoftUpgradeLog(new LoggerFactory().AddSerilog(
            new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()
        ));
    }
}
