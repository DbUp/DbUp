using System;
using System.Data.SQLite;
using System.IO;
using Mono.Data.Sqlite;
using NUnit.Framework;

namespace DbUp.Tests.Support.SQLiteMono
{
    [TestFixture]
    public class SQLiteMonoSupportTests
    {
        private static readonly string dbFilePath = Path.Combine(Environment.CurrentDirectory, "test.db");

        [Test]
        public void CanUseSQLite()
        {
            string connectionString = string.Format("Data Source={0}; Version=3;", dbFilePath);

            if (!File.Exists(dbFilePath))
            {
                SqliteConnection.CreateFile(dbFilePath);
            }

            var upgrader = DeployChanges.To
                .SQLiteMonoDatabase(connectionString)
                .WithScript("Script0001", "CREATE TABLE IF NOT EXISTS Foo (Id int)")
                .Build();
        }
    }
}
