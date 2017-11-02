using System;
using System.Data;
using System.Data.Common;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDbCommand : IDbCommand
    {
        private readonly Action<DatabaseAction> add;
        private readonly bool schemaTableExists;

        public RecordingDbCommand(Action<DatabaseAction> add, bool schemaTableExists)
        {
            this.add = add;
            this.schemaTableExists = schemaTableExists;
            Parameters = new RecordingDataParameterCollection(add);
        }

        public void Dispose()
        {
            add(DatabaseAction.DisposeCommand());
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter CreateParameter()
        {
            add(DatabaseAction.CreateParameter());
            return new RecordingDbDataParameter();
        }

        public int ExecuteNonQuery()
        {
            add(DatabaseAction.ExecuteNonQuery(CommandText));

            if (CommandText == "error")
                ThrowError();
            return 0;
        }

        private void ThrowError()
        {
            throw new TestDbException();
        }

        public IDataReader ExecuteReader()
        {
            add(DatabaseAction.ExecuteReaderCommand(CommandText));

            if (CommandText == "error")
                ThrowError();

            return new DataTableReader(new DataTable());
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar()
        {
            add(DatabaseAction.ExecuteScalarCommand(CommandText));

            if (CommandText == "error" || (CommandText.ToLower().Contains("count") && CommandText.ToLower().Contains("schemaversion") && !schemaTableExists))
                ThrowError();
            return null;
        }

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// Set to 'error' to throw when executed
        /// </summary>
        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDataParameterCollection Parameters { get; private set; }

        public UpdateRowSource UpdatedRowSource { get; set; }

        private class TestDbException : DbException
        {
        }
    }
}