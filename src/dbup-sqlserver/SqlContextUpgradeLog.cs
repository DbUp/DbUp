#if SUPPORTS_SQL_CONTEXT
using Microsoft.SqlServer.Server;

namespace DbUp.Engine.Output
{
    class SqlContextUpgradeLog : IUpgradeLog
    {
        public void WriteInformation(string format, params object[] args)
        {
            SqlContext.Pipe.Send("INFO:  " + string.Format(format, args));
        }

        public void WriteError(string format, params object[] args)
        {
            SqlContext.Pipe.Send("ERROR: " + string.Format(format, args));
        }

        public void WriteWarning(string format, params object[] args)
        {
            SqlContext.Pipe.Send("WARN:  " + string.Format(format, args));
        }
    }
}
#endif