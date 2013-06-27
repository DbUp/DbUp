using System;
using System.Linq;
using System.Reflection;
using DbUp;
using DbUp.Helpers;

namespace SampleApplication
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            using (var database = new TemporarySqlDatabase("SampleApplication"))
            {
                database.Create();

                var upgradeEngineBuilder = DeployChanges.To
                    .SqlDatabase(database.ConnectionString, null) //null or "" for default schema for user
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script =>
                    {
                        if (script.EndsWith("Script0006 - Transactions.sql"))
                            return !args.Any(a => "--noError".Equals(a, StringComparison.InvariantCultureIgnoreCase));

                        return true;
                    })
                    .LogToConsole();

                if (args.Any(a => "--withTransaction".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
                    upgradeEngineBuilder = upgradeEngineBuilder.WithTransaction();
                else if (args.Any(a => "--withTransactionPerScript".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
                    upgradeEngineBuilder = upgradeEngineBuilder.WithTransactionPerScript();

                var upgrader =  upgradeEngineBuilder.Build();

                var result = upgrader.PerformUpgrade();

                // Display the result
                if (result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.WriteLine("Failed!");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("Press any key to delete your database and continue");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Try the --withTransaction or --withTransactionPerScript to see transaction support in action");
                Console.WriteLine("--noError to exclude the broken script");
                Console.ReadKey();
                // Database will be deleted at this point
            }
        }
    }
}
