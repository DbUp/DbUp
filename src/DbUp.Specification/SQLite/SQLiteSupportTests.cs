using System;
using System.Data.SQLite;
using System.IO;
using NUnit.Framework;

namespace DbUp.Tests.SQLite
{
    [TestFixture]
    public class SQLiteSupportTests
    {
        private static readonly string dbFilePath = Path.Combine(Environment.CurrentDirectory, "test.db");

        [Test]
        public void CanUseSQLite()
        {
            string connectionString = string.Format("Data Source={0}; Version=3;", dbFilePath);

            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
            }

            var upgrader = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScript("Script0001", "CREATE TABLE IF NOT EXISTS Foo (Id int)")
                .Build();
        }
    }
}
