using System;
using DbUp;
using DbUp.Execution;
using DbUp.Helpers;
using DbUp.Journal;
using DbUp.ScriptProviders;

namespace SampleApplication
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            using (var database = new TemporarySqlDatabase("SampleApplication"))
            {
                database.Create();

                // Deploy the schema
                var upgrader = new DatabaseUpgrader(
                    database.ConnectionString,
                    new EmbeddedScriptProvider(typeof (Program).Assembly),
                    new TableJournal(),
                    new SqlScriptExecutor()
                    );

                var result = upgrader.PerformUpgrade(new ConsoleLog());

                // Display the result
                if (result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ReadKey();
                    Console.WriteLine("Failed!");
                }

                // Database will be deleted at this point
            }

            Console.ReadKey();
        }
    }
}
