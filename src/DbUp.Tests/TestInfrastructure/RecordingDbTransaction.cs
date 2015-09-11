using System;
using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDbTransaction : IDbTransaction
    {
        private readonly Action<DatabaseAction> recordAction;

        public RecordingDbTransaction(Action<DatabaseAction> recordAction)
        {
            this.recordAction = recordAction;
        }

        public void Dispose()
        {
            recordAction(DatabaseAction.DisposeTransaction());
        }

        public void Commit()
        {
            recordAction(DatabaseAction.CommitTransaction());
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public IDbConnection Connection { get; private set; }
        public IsolationLevel IsolationLevel { get; private set; }
    }
}