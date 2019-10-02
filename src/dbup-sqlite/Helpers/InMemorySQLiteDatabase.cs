using System;
using DbUp.Engine.Transactions;
using DbUp.Helpers;

#if MONO
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
using SQLiteConnectionStringBuilder = Mono.Data.Sqlite.SqliteConnectionStringBuilder;
using SQLiteJournalModeEnum = Mono.Data.Sqlite.SQLiteJournalModeEnum;
#elif NETCORE
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
using SQLiteConnectionStringBuilder = Microsoft.Data.Sqlite.SqliteConnectionStringBuilder;
#else
using System.Data.SQLite;
#endif

namespace DbUp.SQLite.Helpers
{
    /// <summary>
    /// Used to create in-memory SQLite database that is deleted at the end of a test.
    /// </summary>
    public class InMemorySQLiteDatabase : IDisposable
    {
        readonly SQLiteConnectionManager connectionManager;
        readonly SQLiteConnection sharedConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySQLiteDatabase"/> class.
        /// </summary>
        public InMemorySQLiteDatabase()
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = ":memory:",
#if !NETCORE
                Version = 3,
                DefaultTimeout = 5,
#if MONO
                JournalMode = SQLiteJournalModeEnum.Off,
#else
                JournalMode = SQLiteJournalModeEnum.Memory,
#endif
                UseUTF16Encoding = true
#endif
            };
            ConnectionString = connectionStringBuilder.ToString();

            connectionManager = new SQLiteConnectionManager(connectionStringBuilder.ConnectionString);
            sharedConnection = new SQLiteConnection(connectionStringBuilder.ConnectionString);
            sharedConnection.Open();
            SqlRunner = new AdHocSqlRunner(() => sharedConnection.CreateCommand(), new SQLiteObjectParser(), null, () => true);
        }

        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the connection factory of in-memory database.
        /// </summary>
        public IConnectionManager GetConnectionManager() => connectionManager;

        /// <summary>
        /// An adhoc sql runner against the in-memory database
        /// </summary>
        public AdHocSqlRunner SqlRunner { get; }

        /// <summary>
        /// Remove the database from memory.
        /// </summary>
        public void Dispose() => sharedConnection.Dispose();
    }
}
