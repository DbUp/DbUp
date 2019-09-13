using System;
using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    class RecordingDbTransaction : IDbTransaction
    {
        readonly CaptureLogsLogger logger;

        public RecordingDbTransaction(CaptureLogsLogger logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            logger.WriteDbOperation("Dispose transaction");
        }

        public void Commit()
        {
            logger.WriteDbOperation("Commit transaction");
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public IDbConnection Connection { get; private set; }
        public IsolationLevel IsolationLevel { get; private set; }
    }
}