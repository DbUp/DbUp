using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DbUp.Execution;
using DbUp.Journal;
using DbUp.ScriptProviders;
using NDesk.Options;

namespace DbUp.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var server = "";
            var database = "";
            var directory = "";
            var username = "";
            var password = "";


            bool show_help = false;

            var optionSet = new OptionSet() {
            { "s|server=", "the SQL Server host", s => server = s },
            { "db|database=", "database to upgrade", d => database = d},
            { "d|directory=", "directory containing SQL Update files", dir => directory = dir },
            { "u|user=", "Database username", u => username = u},
            { "p|password=", "Database password", p => password = p},
            { "h|help",  "show this message and exit", v => show_help = v != null },
        };

            optionSet.Parse(args);

            if (args.Length == 0)
                show_help = true;


            if ( show_help )
            {
                optionSet.WriteOptionDescriptions(System.Console.Out);
                return;
                
            }
            string connectionString = BuildConnectionString(server, database, username, password);


            var dbup = new DatabaseUpgrader(connectionString, new FileSystemScriptProvider(directory),
                                            new TableJournal(connectionString), new SqlScriptExecutor(connectionString),
                                            new ConsoleLog());
            dbup.PerformUpgrade();

        }

        private static string BuildConnectionString(string server, string database, string username, string password)
        {
            var conn = new SqlConnectionStringBuilder();
            conn.DataSource = server;
            conn.InitialCatalog = database;
            if (!String.IsNullOrEmpty(username))
            {
                conn.UserID = username;
                conn.Password = password;
                conn.IntegratedSecurity = false;
            }
            else
            {
                conn.IntegratedSecurity = true;
            }

            return conn.ToString();
        }
    }
}
