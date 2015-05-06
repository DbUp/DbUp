using System;
using System.IO;
using DbUp.Helpers;
using Mono.Data.Sqlite;

namespace DbUp.SQLite.Mono.Helpers
{
    /// <summary>
    /// Used to create SQLite databases that are deleted at the end of a test.
    /// </summary>
    public class TemporarySQLiteDatabase : IDisposable
    {
        private readonly string dataSourcePath;
        private readonly AdHocSqlRunner sqlRunner;
        private readonly SqliteConnection sqLiteConnection;
        private readonly SharedConnection sharedConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySQLiteDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySQLiteDatabase(string name)
        {
            dataSourcePath = Path.Combine(Environment.CurrentDirectory, name);

            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = name,
                Version = 3,
                DefaultTimeout = 5,
                JournalMode = SQLiteJournalModeEnum.Delete,
                UseUTF16Encoding = true
            };

            sqLiteConnection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            sqLiteConnection.Open();
            sharedConnection = new SharedConnection(sqLiteConnection);
            sqlRunner = new AdHocSqlRunner(() => sqLiteConnection.CreateCommand(), null, () => true);
        }

        /// <summary>
        /// An ahoc sql runner against the temporary database
        /// </summary>
        public AdHocSqlRunner SqlRunner
        {
            get { return sqlRunner; }
        }

        public SharedConnection SharedConnection
        {
            get { return sharedConnection; }
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        public void Create()
        {
            var filePath = new FileInfo(dataSourcePath);
            if (!filePath.Exists)
            {
                SqliteConnection.CreateFile(dataSourcePath);
            }
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void Dispose()
        {
            var filePath = new FileInfo(dataSourcePath);
            if (!filePath.Exists) return;
            sharedConnection.Dispose();
            sqLiteConnection.Dispose();
            SqliteConnection.ClearAllPools();

            // SQLite requires all created sql connection/command objects to be disposed
            // in order to delete the database file
            GC.Collect(2, GCCollectionMode.Forced);
            System.Threading.Thread.Sleep(100);

            File.Delete(dataSourcePath);
        }
    }
}
