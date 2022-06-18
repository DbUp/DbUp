using System;

namespace SQLiteSampleApplication
{
    public static class Program
    {
        static void Main()
        {
            InMemoryDb();
            TemporaryFileDb();
            PermanentFileDb();
        }

        static void InMemoryDb()
        {
            using (var database = new DbUp.SQLite.Helpers.InMemorySQLiteDatabase())
            {
                var upgrader =
                    DbUp.DeployChanges.To
                        .SQLiteDatabase(database.ConnectionString)
                        .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
                        .LogToConsole()
                        .Build();

                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                DbUp.Engine.DatabaseUpgradeResult result = upgrader.PerformUpgrade();
                watch.Stop();

                Display("InMemory", result, watch.Elapsed);
            } // Database will be deleted at this point
        }

        static void TemporaryFileDb()
        {
            using (var database = new DbUp.SQLite.Helpers.TemporarySQLiteDatabase("test.db"))
            {
                var upgrader =
                    DbUp.DeployChanges.To
                        .SQLiteDatabase(database.SharedConnection)
                        .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
                        .LogToConsole()
                        .Build();

                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                DbUp.Engine.DatabaseUpgradeResult result = upgrader.PerformUpgrade();
                watch.Stop();

                Display("Temporary file", result, watch.Elapsed);
            } // Database will be deleted at this point
        }

        static void PermanentFileDb()
        {
            Microsoft.Data.Sqlite.SqliteConnection connection = new("Data Source=dbup.db");

            using (var database = new DbUp.SQLite.Helpers.SharedConnection(connection))
            {
                var upgrader = DbUp.DeployChanges
                    .To
                    .SQLiteDatabase(connection.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                DbUp.Engine.DatabaseUpgradeResult result = upgrader.PerformUpgrade();
                watch.Stop();

                Display("Permanent file", result, watch.Elapsed);
            } // Database will NOT be deleted at this point
        }

        static void Display(string dbType, DbUp.Engine.DatabaseUpgradeResult result, TimeSpan ts)
        {
            // Display the result
            if (result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Console.WriteLine(
                    "{0} Database Upgrade Runtime: {1}",
                    dbType,
                    string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));
                Console.ReadKey();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ReadKey();
                Console.WriteLine("Failed!");
            }
        }
    }
}