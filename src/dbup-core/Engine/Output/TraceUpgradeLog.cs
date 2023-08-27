using System;
using System.Diagnostics;

namespace DbUp.Engine.Output
{
    /// <summary>
    /// A log that writes to System.Diagnostics.Trace.
    /// </summary>
    public class TraceUpgradeLog : IUpgradeLog
    {
        /// <inheritdoc/>
        public void LogTrace(string format, params object[] args)
            => Log(Constants.TraceLevel, format, args);

        /// <inheritdoc/>
        public void LogDebug(string format, params object[] args)
            => Log(Constants.DebugLevel, format, args);

        /// <inheritdoc/>
        public void LogInformation(string format, params object[] args)
            => Log(Constants.InfoLevel, format, args);

        /// <inheritdoc/>
        public void LogWarning(string format, params object[] args)
            => Log(Constants.WarnLevel, format, args);

        /// <inheritdoc/>
        public void LogError(string format, params object[] args)
            => Log(Constants.ErrorLevel, format, args);

        /// <inheritdoc/>
        public void LogError(Exception ex, string format, params object[] args)
            => Log(Constants.ErrorLevel, format, args, ex);

        static void Log(string level, string format, object[] args, Exception ex = null)
        {
            Trace.WriteLine($"{DateTimeOffset.Now.ToString(Constants.TimestampFormat)} [{level}] {string.Format(format, args)}");
            if (ex != null)
            {
                Trace.WriteLine(ExceptionFormatter.Format(ex));
            }
        }
    }
}
