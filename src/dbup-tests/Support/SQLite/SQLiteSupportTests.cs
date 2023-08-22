using System;
using System.Data.SQLite;
using System.IO;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.SQLite
{
    public class SQLiteSupportTests
    {
        static readonly string dbFilePath = Path.Combine(Environment.CurrentDirectory, "test.db");

        [Fact]
        public void CanUseSQLite()
        {
            var connectionString = $"Data Source={dbFilePath}; Version=3;";

            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
            }

            var upgrader = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScript("Script0001", "CREATE TABLE IF NOT EXISTS Foo (Id int)")
                .Build();
        }

        [Fact]
        public void DoesNotExhibitSafeHandleError_Issue577()
        {
            var connectionString = "Data source=:memory:";

            var upgrader =
                DeployChanges.To
                    .SQLiteDatabase(connectionString)
                    .WithScript("Script001", @"
create table test (
    contact_id INTEGER PRIMARY KEY
);
")
                    .LogScriptOutput()
                    .LogToConsole()
                    .Build();
            var result = upgrader.PerformUpgrade();
            result.Successful.ShouldBeTrue();
        }
    }
}
