using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DbUp;

namespace FirebirdSampleApplication
{
    class Program
    {
        static int Main()
        {
            var config = GetConfig();
            string connectionString = config.GetConnectionString("SampleFirebird");

            var upgrader =
                DeployChanges.To
                    .FirebirdDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .WithTransactionPerScript()
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

                WaitIfDebug();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            WaitIfDebug();
            return 0;
        }

        private static IConfiguration GetConfig()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            return config;
        }

        [Conditional("DEBUG")]
        public static void WaitIfDebug()
        {
            Console.ReadLine();
        }
    }
}
