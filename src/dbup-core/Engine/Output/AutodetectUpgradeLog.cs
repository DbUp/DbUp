#if SUPPORTS_LIBLOG
using DbUp.Engine.Output.LibLog;

namespace DbUp.Engine.Output
{
    public class AutodetectUpgradeLog : IUpgradeLog
    {
        private readonly Logger log = LogProvider.ForceResolveLogProvider().GetLogger("DbUp");

        public void WriteInformation(string format, params object[] args) => log(LogLevel.Info, () => format, null, args);

        public void WriteError(string format, params object[] args) => log(LogLevel.Error, () => format, null, args);

        public void WriteWarning(string format, params object[] args) => log(LogLevel.Warn, () => format, null, args);
    }
}
#endif