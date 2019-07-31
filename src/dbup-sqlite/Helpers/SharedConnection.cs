using System;
using System.Data;

namespace DbUp.SQLite.Helpers
{
    /// <summary>
    /// A database connection wrapper to manage underlying connection as a shared connection
    /// during database upgrade. 
    /// <remarks>
    /// if underlying connection is already opened then it will be kept as opened and will not be closed 
    /// otherwise it will be opened when object is created and closed when object is disposed
    /// however it will not be disposed
    /// </remarks>
    /// </summary>
    public class SharedConnection : IDbConnection
    {
        private readonly bool connectionAlreadyOpenned;
        private readonly IDbConnection connection;

        /// <summary>
        /// Constructs a new instance
        /// </summary>
        public SharedConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                throw new NullReferenceException("database connection is null");
            connection = dbConnection;

            if (connection.State == ConnectionState.Open)
                connectionAlreadyOpenned = true;
            else
                connection.Open();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return connection.BeginTransaction(il);
        }

        public IDbTransaction BeginTransaction()
        {
            return connection.BeginTransaction();
        }

        public void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            // shared underlying connection is not closed 
        }

        public string ConnectionString
        {
            get { return connection.ConnectionString; }
            set { connection.ConnectionString = value; }
        }

        public int ConnectionTimeout
        {
            get { return connection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            return connection.CreateCommand();
        }

        public string Database
        {
            get { return connection.Database; }
        }

        public void Open()
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public ConnectionState State
        {
            get { return connection.State; }
        }

        public void Dispose()
        {
            // shared underlying connection is not disposed
        }

        public void DoClose()
        {
            // if shared underlying connection is opened by this object
            // it will be closed here, otherwise the connection is not closed 
            if (!connectionAlreadyOpenned &&
                connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}