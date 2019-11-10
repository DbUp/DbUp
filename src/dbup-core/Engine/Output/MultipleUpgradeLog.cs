using System.Linq;

namespace DbUp.Engine.Output
{
    public class MultipleUpgradeLog : IUpgradeLog
    {
        readonly IUpgradeLog[] upgradeLogs;

        public MultipleUpgradeLog(params IUpgradeLog[] upgradeLogs)
        {
            var otherMultipleLogs = upgradeLogs.OfType<MultipleUpgradeLog>().ToArray();

            this.upgradeLogs = upgradeLogs
                .Except(otherMultipleLogs)
                .Concat(otherMultipleLogs.SelectMany(l => l.upgradeLogs))
                .ToArray();
        }

        public void WriteInformation(string format, params object[] args)
        {
            foreach (var log in upgradeLogs)
                log.WriteInformation(format, args);
        }

        public void WriteError(string format, params object[] args)
        {
            foreach (var log in upgradeLogs)
                log.WriteError(format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            foreach (var log in upgradeLogs)
                log.WriteWarning(format, args);
        }
    }
}