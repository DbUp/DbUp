#if SUPPORTS_LIBLOG
using System;
using DbUp.Engine.Output.LibLog;

namespace DbUp.Engine.Output
{
    public class AutodetectUpgradeLog : IUpgradeLog
    {
        readonly Logger log = LogProvider.ForceResolveLogProvider()?.GetLogger("DbUp")
                                      ?? LogToConsoleInstead;

        public void WriteInformation(string format, params object[] args) => log(LogLevel.Info, () => format, null, args);

        public void WriteError(string format, params object[] args) => log(LogLevel.Error, () => format, null, args);

        public void WriteWarning(string format, params object[] args) => log(LogLevel.Warn, () => format, null, args);

        static bool LogToConsoleInstead(LogLevel level, Func<string> format, Exception exception, object[] args)
        {
            ConsoleColor GetColor()
            {
                switch (level)
                {
                    case LogLevel.Warn:
                        return ConsoleColor.Yellow;
                    case LogLevel.Fatal:
                    case LogLevel.Error:
                        return ConsoleColor.Red;
                    default:
                        return ConsoleColor.White;
                }
            }

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColor();
            try
            {
                Console.WriteLine(format(), args);
            }
            finally
            {
                Console.ForegroundColor = oldColor;
            }
            return true;
        }
    }
}
#endif