using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using DbUp;

namespace FirebirdSampleApplication
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SampleFirebird"].ConnectionString;



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

        [Conditional("DEBUG")]
        public static void WaitIfDebug()
        {
            Console.ReadLine();
        }
    }
}
