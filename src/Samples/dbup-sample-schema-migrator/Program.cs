using System;
using System.Diagnostics;
using Microsoft.Extensions.CommandLineUtils;

namespace DbUp.Sample.SchemaMigrator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication
            {
                Name = "SchemaMigrator.exe",
                FullName = "DbUp Migrations"
            };

            var appVersion = typeof(Program).Assembly.GetName().Version.ToString();
            var dbUpVersion = typeof(DeployChanges).Assembly.GetName().Version.ToString();

            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", () => $"{appVersion} using DbUp {dbUpVersion}");
            app.Command("migrate", MigrateCommand);

            try
            {
                // if no subcommand/option was specified, display help
                app.OnExecute(() =>
                {
                    app.ShowHelp();
                    return 2;
                });

                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fatal. DbUp failed to apply migrations. {ex.Message}");
                Console.WriteLine("\n\nIf you are seeing the same error after modifying scripts? Try force rebuild the project to ensure latest scripts have been embedded.\n");

                app.ShowHelp();

                return 1;
            }
            finally
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("\nPress any key to close...");
                    Console.ReadKey(true);
                }
#endif
            }
        }

        private static void MigrateCommand(Microsoft.Extensions.CommandLineUtils.CommandLineApplication c)
        {
            c.Description = "Migrate schema, targeting SQL Server";

            var argConnectionString = c.Argument("<connectionString>", "SQL Server Connection String");
            var optionEnsureDatabaseIsCreated = c.Option("--create", "Ensure the database is created before migration", CommandOptionType.NoValue);
            var optionStableScriptsOnly = c.Option("--stable", "Only execute stable scripts", CommandOptionType.NoValue);

            c.HelpOption("-?|-h|--help");

            c.OnExecute(() =>
            {
                var connectionString = argConnectionString.Value;
                var ensureDatabaseIsCreated = optionEnsureDatabaseIsCreated.HasValue();
                var stableScriptsOnly = optionStableScriptsOnly.HasValue();

                var migrator = new SqlServerMigrator(connectionString, stableScriptsOnly);
                if (ensureDatabaseIsCreated)
                    migrator.Create();

                var result = migrator.PerformUpgrade();

                if (result.Successful)
                    return 0;

                throw result.Error;
            });
        }

    }
}
