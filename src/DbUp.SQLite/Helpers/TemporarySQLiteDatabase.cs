using System;
using System.IO;
using DbUp.Helpers;
using System.Data.SQLite;

namespace DbUp.SQLite.Helpers
{
    /// <summary>
    /// Used to create SQLite databases that are deleted at the end of a test.
    /// </summary>
    public class TemporarySQLiteDatabase : IDisposable
    {
        private readonly string dataSourcePath;
        private readonly string connectionString;
        private readonly AdHocSqlRunner sqlRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySQLiteDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySQLiteDatabase(string name)
        {
            dataSourcePath = Path.Combine(Environment.CurrentDirectory, name);

            var connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = name;
            connectionStringBuilder.Version = 3;
            connectionStringBuilder.DefaultTimeout = 5;
            connectionStringBuilder.JournalMode = SQLiteJournalModeEnum.Memory;
            connectionStringBuilder.UseUTF16Encoding = true;

            connectionString = connectionStringBuilder.ConnectionString;
            sqlRunner = new AdHocSqlRunner(() => new SQLiteConnection(connectionString), null, () => true);
        }

        /// <summary>
        /// Gets the connection string of temporary database.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
        }

        /// <summary>
        /// An ahoc sql runner against the temporary database
        /// </summary>
        public AdHocSqlRunner SqlRunner
        {
            get { return sqlRunner; }
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
            if (filePath.Exists)
            {
                SQLiteConnection.ClearAllPools();

                // SQLite requires all created sql connection/command objects to be disposed
                // in order to delete the database file
                GC.Collect(2, GCCollectionMode.Forced);
                System.Threading.Thread.Sleep(100);

                File.Delete(dataSourcePath);
            }
        }
    }
}
