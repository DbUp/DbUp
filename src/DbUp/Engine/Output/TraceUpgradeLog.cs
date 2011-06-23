using System.Diagnostics;

namespace DbUp.Engine.Output
{
    /// <summary>
    /// A log that writes to System.Diagnostics.Trace.
    /// </summary>
    public class TraceUpgradeLog : IUpgradeLog
    {
        public void WriteInformation(string format, params object[] args)
        {
            Trace.WriteLine("INFO:  " + string.Format(format, args));
        }

        public void WriteError(string format, params object[] args)
        {
            Trace.WriteLine("ERROR: " + string.Format(format, args));
        }

        public void WriteWarning(string format, params object[] args)
        {
            Trace.WriteLine("WARN:  " + string.Format(format, args));
        }
    }
}