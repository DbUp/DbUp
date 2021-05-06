using System;
using System.Diagnostics;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using DbUp.SQLite.Helpers;

namespace SQLiteSampleApplication
{
    class Program
    {
        static void Main()
        {
            using (var database = new TemporarySQLiteDatabase("test"))
            {
                database.Create();

                var upgrader =
                    DeployChanges.To
                    .SQLiteDatabase(database.SharedConnection)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                var watch = new Stopwatch();
                watch.Start();

                var result = upgrader.PerformUpgrade();

                watch.Stop();
                Display("File", result, watch.Elapsed);
            } // Database will be deleted at this point

            using (var database = new InMemorySQLiteDatabase())
            {
                var upgrader =
                    DeployChanges.To
                    .SQLiteDatabase(database.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                var watch = new Stopwatch();
                watch.Start();

                var result = upgrader.PerformUpgrade();

                watch.Stop();
                Display("InMemory", result, watch.Elapsed);
            } // Database will disappear from memory at this point
        }

        static void Display(string dbType, DatabaseUpgradeResult result, TimeSpan ts)
        {
            // Display the result
            if (result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Console.WriteLine("{0} Database Upgrade Runtime: {1}", dbType,
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
