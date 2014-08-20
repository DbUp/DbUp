using System;
using System.Data.SqlClient;
using System.Diagnostics;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using NDesk.Options;

namespace DbUp.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var directory = "";
            bool mark = false;
            var connectionString = "";
            SupportedDatabases.Type selectedDbMs = SupportedDatabases.Type.MsSql;
            TransactionMode transactionMode = TransactionMode.SingleTransaction;

            bool showHelp = false;

            var optionSet = new OptionSet() {
                { "dbms|databaseMs=", "Database managment system to use (0 = MSSQL, 1=SQLite, 2=Oracle). Default Database managment system is MSSQL.", dbMs => selectedDbMs = SupportedDatabases.Type.Parse(Convert.ToInt32(dbMs))},
                { "cs|connectionString=", "Full connection string to database", cs => connectionString = cs},
                { "d|directory=", "Directory containing SQL Update files", dir => directory = dir },
                { "tran|transactionMode=", "Use per script transaction mode", tMode => transactionMode = tMode.Equals("true", StringComparison.InvariantCultureIgnoreCase)? TransactionMode.TransactionPerScript : TransactionMode.SingleTransaction},
                { "h|help", "Show this message and exit", v => showHelp = v != null },
                {"mark", "Mark scripts as executed but take no action", m => mark = true},
            };

            optionSet.Parse(args);

            if (args.Length == 0)
                showHelp = true;


            if (showHelp)
            {
                optionSet.WriteOptionDescriptions(System.Console.Out);
                return 2;

            }
            if (String.IsNullOrEmpty(connectionString))
            {
                System.Console.WriteLine("No connection string was passed!");
                optionSet.WriteOptionDescriptions(System.Console.Out);
                return 2;
            }

            UpgradeEngineBuilder engineFactoryBuilder = null;

            switch (selectedDbMs)
            {
                case SupportedDatabases.Type.TypeValue.MsSql:
                    engineFactoryBuilder = DeployChanges.To.SqlDatabase(connectionString);
                    break;
                case SupportedDatabases.Type.TypeValue.Sqlite:
                    engineFactoryBuilder = DeployChanges.To.SQLiteDatabase(connectionString);
                    break;
                case SupportedDatabases.Type.TypeValue.Oracle:
                    // TODO: Naredi še za Oracle database
                    break;
            }

            engineFactoryBuilder = engineFactoryBuilder
                .LogToConsole()
                .WithScriptsFromFileSystemAbsolute(directory);

            if (transactionMode ==TransactionMode.SingleTransaction)
                engineFactoryBuilder.WithTransaction();
            
            else if (transactionMode == TransactionMode.TransactionPerScript)
                engineFactoryBuilder.WithTransactionPerScript();
            
            else
                engineFactoryBuilder.WithoutTransaction();

            UpgradeEngine dbup = engineFactoryBuilder.Build();

            DatabaseUpgradeResult results = !mark ? dbup.PerformUpgrade() : dbup.MarkAsExecuted();
            // Display the result
            if (results.Successful)
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Success!");
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Failed!");
                return 1;
            }
            return 0;
        }
    }
}
