using System;
using System.IO;
using DbUp.Helpers;
using System.Data.SQLite;
using DbUp.Support.SqlServer;

namespace DbUp.SQLite.Helpers
{
    /// <summary>
    /// Used to create SQLite databases that are deleted at the end of a test.
    /// </summary>
    public class TemporarySQLiteDatabase : IDisposable
    {
        private readonly string dataSourcePath;
        private readonly AdHocSqlRunner sqlRunner;
        private readonly SQLiteConnection sqLiteConnection;
        private readonly SharedConnection sharedConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySQLiteDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySQLiteDatabase(string name)
        {
            dataSourcePath = Path.Combine(Environment.CurrentDirectory, name);

            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = name,
                Version = 3,
                DefaultTimeout = 5,
                JournalMode = SQLiteJournalModeEnum.Memory,
                UseUTF16Encoding = true
            };

            sqLiteConnection = new SQLiteConnection(connectionStringBuilder.ConnectionString);
            sharedConnection = new SharedConnection(sqLiteConnection.OpenAndReturn());
            sqlRunner = new AdHocSqlRunner(() => sqLiteConnection.CreateCommand(), null, new SqlObjectParser(), () => true);
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
                SQLiteConnection.CreateFile(dataSourcePath);
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
            SQLiteConnection.ClearAllPools();

            // SQLite requires all created sql connection/command objects to be disposed
            // in order to delete the database file
            GC.Collect(2, GCCollectionMode.Forced);
            System.Threading.Thread.Sleep(100);

            File.Delete(dataSourcePath);
        }
    }
}
