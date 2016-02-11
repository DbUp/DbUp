using System;
using System.Collections.Generic;
using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDbConnection : IDbConnection
    {
        readonly CaptureLogsLogger logger;
        readonly bool schemaTableExists;
        readonly string schemaTableName;

        public RecordingDbConnection(CaptureLogsLogger logger, bool schemaTableExists, string schemaTableName)
        {
            this.logger = logger;
            this.schemaTableExists = schemaTableExists;
            this.schemaTableName = schemaTableName;
        }

        public IDbTransaction BeginTransaction()
        {
            logger.WriteDbOperation("Begin transaction");
            return new RecordingDbTransaction(logger);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            logger.WriteDbOperation(string.Format("Begin transaction with isolationLevel of {0}", il));
            return new RecordingDbTransaction(logger);
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            return new RecordingDbCommand(logger, schemaTableExists, schemaTableName);
        }

        public void Open()
        {
            logger.WriteDbOperation("Open connection");
        }

        public void Dispose()
        {
            logger.WriteDbOperation("Dispose connection");
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; private set; }

        public string Database { get; private set; }

        public ConnectionState State { get; private set; }
    }
}