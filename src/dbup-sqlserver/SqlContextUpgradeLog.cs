#if SUPPORTS_SQL_CONTEXT
using System;
using Microsoft.SqlServer.Server;

namespace DbUp.Engine.Output
{
    class SqlContextUpgradeLog : IUpgradeLog
    {
        public void LogTrace(string format, params object[] args)
            => Log("TRACE", format, args);

        public void LogDebug(string format, params object[] args)
            => Log("DEBUG", format, args);

        public void LogInformation(string format, params object[] args)
            => Log("INFO", format, args);

        public void LogWarning(string format, params object[] args)
            => Log("WARN", format, args);

        public void LogError(string format, params object[] args)
            => Log("ERROR", format, args);

        public void LogError(Exception ex, string format, params object[] args)
            => Log("ERROR", format, args, ex);

        static void Log(string level, string format, object[] args, Exception ex = null)
            => SqlContext.Pipe.Send($"{level}: {string.Format(format, args)}{(ex is not null ? $" => {ex.Message}" : "")}");
    }
}
#endif
