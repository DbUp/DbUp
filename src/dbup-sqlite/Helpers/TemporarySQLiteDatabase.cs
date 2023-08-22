﻿using System;
using System.Data.SQLite;
using System.IO;
using DbUp.Helpers;

namespace DbUp.SQLite.Helpers
{
    /// <summary>
    /// Used to create SQLite databases that are deleted at the end of a test.
    /// </summary>
    public class TemporarySQLiteDatabase : IDisposable
    {
        readonly string dataSourcePath;
        readonly SQLiteConnection sqLiteConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySQLiteDatabase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TemporarySQLiteDatabase(string name)
        {
            dataSourcePath = Path.Combine(Directory.GetCurrentDirectory(), name);

            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = name,
            };

            sqLiteConnection = new SQLiteConnection(connectionStringBuilder.ConnectionString);
            sqLiteConnection.Open();
            SharedConnection = new SharedConnection(sqLiteConnection);
            SqlRunner = new AdHocSqlRunner(() => sqLiteConnection.CreateCommand(), new SQLiteObjectParser(), null, () => true);
        }

        /// <summary>
        /// An adhoc sql runner against the temporary database
        /// </summary>
        public AdHocSqlRunner SqlRunner { get; }

        public SharedConnection SharedConnection { get; }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void Dispose()
        {
            var filePath = new FileInfo(dataSourcePath);
            if (!filePath.Exists) return;
            SharedConnection.Dispose();
            sqLiteConnection.Dispose();
#if !NETCORE
            SQLiteConnection.ClearAllPools();

            // SQLite requires all created sql connection/command objects to be disposed
            // in order to delete the database file
            GC.Collect(2, GCCollectionMode.Forced);
            System.Threading.Thread.Sleep(100);
#endif
            File.Delete(dataSourcePath);
        }
    }
}
