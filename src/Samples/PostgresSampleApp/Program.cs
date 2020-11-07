using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using DbUp;
using DbUp.Engine.Output;
using DbUp.Helpers;

namespace PostgresSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(DisplayError);
        }

        static void Run(Options options)
        {
            var logger = new ConsoleUpgradeLog();
            
            if (options.RecreateDatabase)
            {
                DropDatabase.For.PostgresqlDatabase(options.ConnectionString, logger);
            }
            
            EnsureDatabase.For.PostgresqlDatabase(options.ConnectionString, logger);

            var upgradeBuilder = DeployChanges
                .To
                .PostgresqlDatabase(options.ConnectionString)
                .LogTo(logger)
                .WithTransactionPerScript()
                .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
                .Build();

            if (options.UpgradeReport)
            {
                var reportFileName = GetTimestampedFileName();
                var path = Path.Join(Directory.GetCurrentDirectory(), reportFileName);
                
                upgradeBuilder.GenerateUpgradeHtmlReport(path);
            }

            if (!options.DryRun)
            {
                var result = upgradeBuilder.PerformUpgrade();
                if (result.Successful)
                {
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.WriteLine(result.Error);
                    Console.WriteLine("Failure.");
                    Environment.Exit(1);
                }
            }
            
            Environment.Exit(0);
        }

        static void DisplayError(IEnumerable<Error> errors)
        {
            Environment.Exit(1);
        }

        static string GetTimestampedFileName()
        {
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");
            return string.Concat("upgrade-report-", now, ".html");
        }
    }
}
