using System;

namespace DbUp
{
    public interface ILog
    {
        void WriteInformation(string format, params object[] args);
        void WriteError(string format, params object[] args);
        void WriteWarning(string format, params object[] args);
    }
}
