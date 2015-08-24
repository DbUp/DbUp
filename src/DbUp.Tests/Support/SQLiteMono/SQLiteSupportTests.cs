using System;
using System.IO;
using DbUp.SQLite.Mono.Helpers;
using Mono.Data.Sqlite;
using NUnit.Framework;

namespace DbUp.Tests.Support.SQLiteMono
{
    [TestFixture]
    public class SQLiteMonoSupportTests
    {
        private static readonly string dbFilePath = Path.Combine(Environment.CurrentDirectory, "test.db");

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            // SqlRepository requires sqllite3 native binary in order to work.
            // So ensure this is copied into the application directory before tests run.
            var currentDir = Environment.CurrentDirectory;
            string destination = Path.Combine(currentDir, "sqlite3.dll");
            if (!File.Exists(destination))
            {
                var packageDir = Path.Combine(currentDir, @"..\..\..\packages\sqlite3");
                string sourceFileName = Path.Combine(packageDir, "sqlite3.dll");
                File.Copy(sourceFileName, destination);
            }
        }

        [Test]
        public void can_use_sql_lite_mono()
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
        

        [Test]
        public void can_perform_adhoc_executereader_and_also_still_delete_the_database()
        {
            string databaseName = "issuetest";
            using (var database = new TemporarySQLiteDatabase(databaseName))
            {
                database.Create();

                string connectionString = database.SharedConnection.ConnectionString;  //string.Format("Data Source={0}; Version=3;", dbFilePath);
                

                var upgrader = DeployChanges.To
                .SQLiteMonoDatabase(connectionString)
                .WithScript("Script0002", "CREATE TABLE IF NOT EXISTS Bar (Id int)")
                .Build();

                var result = upgrader.PerformUpgrade();
                Assert.IsTrue(result.Successful);
                // Calling execute reader on the adhoc sql runner causes a datareader to be created - and if not disposed of,
                // then the when TemporarySQLiteDatabase is disposed, it can't delete the file.
                var lastVersion = database.SqlRunner.ExecuteReader("SELECT * FROM SchemaVersions ORDER BY SchemaVersionId DESC LIMIT 1");
                var appliedScriptRusult = lastVersion[0];

               // Assert.That(lastVersion[0].Values[""].ToString().EndsWith("Script0002"));


            }
        }
    }
}
