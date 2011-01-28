using System;
using DbUp;
using DbUp.Execution;
using DbUp.Journal;
using DbUp.ScriptProviders;

namespace SampleApplication
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            const string connectionString = "server=(local)\\SQLEXPRESS;database=SampleApplication;trusted_connection=true";
            var upgrader = new DatabaseUpgrader(
                connectionString,
                new EmbeddedScriptProvider(typeof (Program).Assembly),
                new TableJournal(),
                new SqlScriptExecutor()
                );

            var result = upgrader.PerformUpgrade(new ConsoleLog());

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

            Console.ReadKey();
        }
    }
}
