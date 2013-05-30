using System;
using System.Data;
using System.Data.SQLite;
using DbUp.Helpers;

namespace DbUp.Sqlite.Helpers
{
    /// <summary>
    /// Used to create in-memory SQLite database that is deleted at the end of a test.
    /// </summary>
    public class InMemorySQLiteDatabase : IDisposable
    {
        private readonly SQLiteConnectionAdapter sharedConnection;
        private readonly AdHocSqlRunner sqlRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySQLiteDatabase"/> class.
        /// </summary>
        public InMemorySQLiteDatabase()
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = ":memory:";
            connectionStringBuilder.Version = 3;
            connectionStringBuilder.DefaultTimeout = 5;
            connectionStringBuilder.JournalMode = SQLiteJournalModeEnum.Memory;
            connectionStringBuilder.UseUTF16Encoding = true;

            sharedConnection = new SQLiteConnectionAdapter(new SQLiteConnection(connectionStringBuilder.ConnectionString));
            sqlRunner = new AdHocSqlRunner(GetConnectionFactory(), null, () => true);
        }

        /// <summary>
        /// Gets the connection factory of in-memory database.
        /// </summary>
        public Func<IDbConnection> GetConnectionFactory()
        {
            return () => (IDbConnection)sharedConnection;
        }

        /// <summary>
        /// An ahoc sql runner against the in-memory database
        /// </summary>
        public AdHocSqlRunner SqlRunner
        {
            get { return sqlRunner; }
        }

        /// <summary>
        /// remove the database from memory
        /// </summary>
        public void Dispose()
        {
            sharedConnection.DisposeConnection();
        }

        /// <summary>
        /// SQLite in-memory database ceases to exist when database connection is closed, 
        /// hence SQLite connection is wrapped using this class to prevent it to be closed 
        /// until the database is explicitly disposed.
        /// </summary>
        private class SQLiteConnectionAdapter : IDbConnection
        {
            private readonly SQLiteConnection sqliteConnection;
            
            public SQLiteConnectionAdapter(SQLiteConnection sqliteConnection)
            {
                this.sqliteConnection = sqliteConnection;
            }

            public IDbTransaction BeginTransaction(IsolationLevel il)
            {
                return sqliteConnection.BeginTransaction(il);
            }

            public IDbTransaction BeginTransaction()
            {
                return sqliteConnection.BeginTransaction();
            }

            public void ChangeDatabase(string databaseName)
            {
                sqliteConnection.ChangeDatabase(databaseName);
            }

            public void Open()
            {
                if (sqliteConnection.State == ConnectionState.Closed)
                    sqliteConnection.Open();
            }

            public void Close()
            {
                // don't close connection becasue it ceases the cache database existance
            }

            public string ConnectionString
            {
                get
                {
                    return sqliteConnection.ConnectionString;
                }
                set
                {
                    sqliteConnection.ConnectionString = value;
                }
            }

            public int ConnectionTimeout
            {
                get { return sqliteConnection.ConnectionTimeout; }
            }

            public IDbCommand CreateCommand()
            {
                return sqliteConnection.CreateCommand();
            }

            public string Database
            {
                get { return sqliteConnection.Database; }
            }

            public ConnectionState State
            {
                get { return sqliteConnection.State; }
            }

            public void Dispose()
            {
                // don't close or dispose the connection becasue it ceases the cache database existance
            }

            public void DisposeConnection()
            {
                // Close and dispose sqlite connection when this method is called
                try
                {
                    sqliteConnection.Close();
                }
                finally
                {
                    sqliteConnection.Dispose();
                }
            }
        }
    }
}
