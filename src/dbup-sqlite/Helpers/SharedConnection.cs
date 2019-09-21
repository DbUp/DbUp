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
        readonly bool connectionAlreadyOpened;
        readonly IDbConnection connection;

        /// <summary>
        /// Constructs a new instance
        /// </summary>
        public SharedConnection(IDbConnection dbConnection)
        {
            connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection), "database connection is null");

            if (connection.State == ConnectionState.Open)
                connectionAlreadyOpened = true;
            else
                connection.Open();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il) => connection.BeginTransaction(il);

        public IDbTransaction BeginTransaction() => connection.BeginTransaction();

        public void ChangeDatabase(string databaseName) => connection.ChangeDatabase(databaseName);

        public void Close() { } // shared underlying connection is not closed 

        public string ConnectionString
        {
            get => connection.ConnectionString;
            set => connection.ConnectionString = value;
        }

        public int ConnectionTimeout => connection.ConnectionTimeout;

        public IDbCommand CreateCommand() => connection.CreateCommand();

        public string Database => connection.Database;

        public void Open()
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public ConnectionState State => connection.State;

        public void Dispose() { } // shared underlying connection is not disposed

        public void DoClose()
        {
            // if shared underlying connection is opened by this object
            // it will be closed here, otherwise the connection is not closed 
            if (!connectionAlreadyOpened && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
