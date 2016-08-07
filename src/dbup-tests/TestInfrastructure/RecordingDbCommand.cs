using System;
using System.Data;
using System.Data.Common;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDbCommand : IDbCommand
    {
        readonly CaptureLogsLogger logger;
        readonly bool schemaTableExists;
        readonly string schemaTableName;

        public RecordingDbCommand(CaptureLogsLogger logger, bool schemaTableExists, string schemaTableName)
        {
            this.logger = logger;
            this.schemaTableExists = schemaTableExists;
            this.schemaTableName = schemaTableName;
            Parameters = new RecordingDataParameterCollection(logger);
        }

        public void Dispose()
        {
            logger.WriteDbOperation("Dispose command");
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
            logger.WriteDbOperation("Create parameter");
            return new RecordingDbDataParameter();
        }

        public int ExecuteNonQuery()
        {
            logger.WriteDbOperation(string.Format("Execute non query command: {0}", CommandText));

            if (CommandText == "error")
                ThrowError();
            return 0;
        }

        void ThrowError()
        {
            throw new TestDbException();
        }

        public IDataReader ExecuteReader()
        {
            logger.WriteDbOperation(string.Format("Execute reader command: {0}", CommandText));

            if (CommandText == "error")
                ThrowError();

            return new EmptyReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar()
        {
            logger.WriteDbOperation(string.Format("Execute scalar command: {0}", CommandText));

            if (CommandText == "error")
                ThrowError();

            // Are we checking if schemaversions exists
            if (CommandText.IndexOf(schemaTableName, StringComparison.OrdinalIgnoreCase) != -1)
            {
                if (schemaTableExists)
                    return 1;
                return 0;
            }

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

        class TestDbException : DbException
        {
        }
    }
}