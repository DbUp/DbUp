using System;
using System.Linq;
using System.Reflection;
using DbUp;
using DbUp.Oracle.Helpers;

namespace OracleSampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"<Oracle connectionString>";
            using (var database = new TemporaryOracleDatabase(connectionString))
            {
                var upgradeEngineBuilder = DeployChanges.To
                    .OracleDatabase(DevartOracleConnectionManager.Instance, database.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script =>
                    {
                        if (script.EndsWith("Script0006 - Transactions.sql"))
                            return !args.Any(a => "--noError".Equals(a, StringComparison.InvariantCultureIgnoreCase));

                        return true;
                    })
                    .LogToConsole();

                var upgrader = upgradeEngineBuilder.Build();

                bool updateRequired = upgrader.IsUpgradeRequired();
                Console.WriteLine("Is upgrade required: " + updateRequired);
                if (updateRequired)
                {
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
                        Console.WriteLine("Failed!");
                    }
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
