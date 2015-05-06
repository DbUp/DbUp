using System;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using Mono.Data.Sqlite;

namespace DbUp.SQLite.Mono.Helpers
{
    /// <summary>
    /// Used to create in-memory SQLite database that is deleted at the end of a test.
    /// </summary>
    public class InMemorySQLiteDatabase : IDisposable
    {
        private readonly SQLiteConnectionManager connectionManager;
        private readonly AdHocSqlRunner sqlRunner;
        private readonly SqliteConnection sharedConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySQLiteDatabase"/> class.
        /// </summary>
        public InMemorySQLiteDatabase()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:",
                Version = 3,
                DefaultTimeout = 5,
                JournalMode = SQLiteJournalModeEnum.Off,  
                UseUTF16Encoding = true
            };
            ConnectionString = connectionStringBuilder.ToString();

            connectionManager = new SQLiteConnectionManager(connectionStringBuilder.ConnectionString);
            sharedConnection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            sharedConnection.Open();
            sqlRunner = new AdHocSqlRunner(() => sharedConnection.CreateCommand(), null, () => true);
        }

        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the connection factory of in-memory database.
        /// </summary>
        public IConnectionManager GetConnectionManager()
        {
            return connectionManager;
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
            sharedConnection.Dispose();
        }
    }
}
