using System;
using System.Data.SQLite;
using DbUp.Engine.Transactions;
using DbUp.Helpers;

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
                JournalMode = SQLiteJournalModeEnum.Memory,
                UseUTF16Encoding = true
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
