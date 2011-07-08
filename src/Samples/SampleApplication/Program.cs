using System;
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

                var upgrader = 
                    DeployChanges.To
                    .SqlDatabase(database.ConnectionString, null) //null or "" for default schema for user
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                var result = upgrader.PerformUpgrade();

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
