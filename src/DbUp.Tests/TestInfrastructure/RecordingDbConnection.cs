using System;
using System.Collections.Generic;
using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDbConnection : IDbConnection
    {
        private readonly bool schemaTableExists;

        public RecordingDbConnection(bool schemaTableExists)
        {
            this.schemaTableExists = schemaTableExists;
        }

        private readonly List<DatabaseAction> dbActions = new List<DatabaseAction>();

        public IDbTransaction BeginTransaction()
        {
            dbActions.Add(DatabaseAction.BeginTransaction());
            return new RecordingDbTransaction(dbActions.Add);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            dbActions.Add(DatabaseAction.BeginTransaction(il));
            return new RecordingDbTransaction(dbActions.Add);
        }

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new System.NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            return new RecordingDbCommand(dbActions.Add, schemaTableExists);
        }

        public void Open()
        {
            dbActions.Add(DatabaseAction.OpenConnection());
        }

        public void Dispose()
        {
            dbActions.Add(DatabaseAction.DisposeConnection());
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; private set; }

        public string Database { get; private set; }

        public ConnectionState State { get; private set; }

        public string GetCommandLog()
        {
            return string.Join(Environment.NewLine, dbActions);
        }
    }
}