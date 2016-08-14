namespace DbUp.Engine.Output
{
    public class NoOpUpgradeLog : IUpgradeLog
    {
        public void WriteInformation(string format, params object[] args)
        {
        }

        public void WriteError(string format, params object[] args)
        {
        }

        public void WriteWarning(string format, params object[] args)
        {
        }
    }
}