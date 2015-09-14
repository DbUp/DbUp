using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp;
using DbUp.Helpers;

namespace SqlServer.FolderRecursion
{
    class Program
    {
        static void Main(string[] args)
        {
            string instanceName = @"(local)";
            string databaseName = @"SampleApplication";
            // Uncomment the following line to run against sql local db instance.
            // string instanceName = @"(localdb)\Projects";

            using (var database = new TemporarySqlDatabase(databaseName, instanceName))
            {
                database.Create();

                var upgradeEngineBuilder = DeployChanges.To
                    .SqlDatabase(database.ConnectionString, null) //null or "" for default schema for user
                    .WithScriptsFromFileSystem(
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Scripts"),
                        filter: script =>
                        {
                            if (script.EndsWith("02 - Transactions.sql"))
                                return !args.Any(a => "--noError".Equals(a, StringComparison.InvariantCultureIgnoreCase));

                            return true;
                        },
                        recursive: true)
                    .WithVariable("DatabaseName", databaseName)
                    .LogToConsole();

                if (args.Any(a => "--withTransaction".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
                    upgradeEngineBuilder = upgradeEngineBuilder.WithTransaction();
                else if (args.Any(a => "--withTransactionPerScript".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
                    upgradeEngineBuilder = upgradeEngineBuilder.WithTransactionPerScript();

                var upgrader = upgradeEngineBuilder.Build();

                Console.WriteLine("Is upgrade required: " + upgrader.IsUpgradeRequired());

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
