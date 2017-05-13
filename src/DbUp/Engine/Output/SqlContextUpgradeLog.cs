using Microsoft.SqlServer.Server;

namespace DbUp.Engine.Output
{
    internal class SqlContextUpgradeLog : IUpgradeLog
    {
        public void WriteInformation(string format, params object[] args)
        {
#if !MONO
            SqlContext.Pipe.Send("INFO:  " + string.Format(format, args));
#endif
        }

        public void WriteError(string format, params object[] args)
        {
#if !MONO
            SqlContext.Pipe.Send("ERROR: " + string.Format(format, args));
#endif
        }

        public void WriteWarning(string format, params object[] args)
        {
#if !MONO
            SqlContext.Pipe.Send("WARN:  " + string.Format(format, args));
#endif
        }
    }
}