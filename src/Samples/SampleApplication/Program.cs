using System;
using System.Linq;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.Support;

namespace SampleApplication
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var instanceName = @"(localdb)\\MSSQLLocalDB";
            // Uncomment the following line to run against sql local db instance.
            // string instanceName = @"(localdb)\Projects";

            var connectionString =
                $"Server={instanceName}; Database=test8; Trusted_connection=true";

            DropDatabase.For.SqlDatabase(connectionString);

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgradeEngineBuilder = DeployChanges.To
                .SqlDatabase(connectionString, null) //null or "" for default schema for user
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script =>
                {
                    if (script.EndsWith("Script0006 - Transactions.sql"))
                        return !args.Any(a => "--noError".Equals(a, StringComparison.InvariantCultureIgnoreCase));

                    return script.StartsWith("SampleApplication.Scripts.");
                })
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.RunAlways."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = DbUpDefaults.DefaultRunGroupOrder + 1 })
                .LogToConsole();

            if (args.Any(a => "--withTransaction".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
            {
                upgradeEngineBuilder = upgradeEngineBuilder.WithTransaction();
            }
            else if (args.Any(a => "--withTransactionPerScript".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
            {
                upgradeEngineBuilder = upgradeEngineBuilder.WithTransactionPerScript();
            }

            var upgrader = upgradeEngineBuilder.Build();

            Console.WriteLine("Is upgrade required: " + upgrader.IsUpgradeRequired());

            if (args.Any(a => "--generateReport".Equals(a, StringComparison.InvariantCultureIgnoreCase)))
            {
                upgrader.GenerateUpgradeHtmlReport("UpgradeReport.html");
            }
            else
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
                    Console.WriteLine(result.Error);
                    Console.WriteLine("Failed!");
                }
            }


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Press any key to delete your database and continue");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(
                "Try the --withTransaction or --withTransactionPerScript to see transaction support in action");
            Console.WriteLine("--noError to exclude the broken script");
            Console.ReadKey();
            // Database will be deleted at this point
        }
    }
}
