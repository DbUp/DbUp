using System;
using System.Data.SqlClient;
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
            bool mark = false;
            var connectionString = "";

            bool show_help = false;

            var optionSet = new OptionSet() {
                { "s|server=", "the SQL Server host", s => server = s },
                { "db|database=", "database to upgrade", d => database = d},
                { "d|directory=", "directory containing SQL Update files", dir => directory = dir },
                { "u|user=", "Database username", u => username = u},
                { "p|password=", "Database password", p => password = p},
                { "cs|connectionString=", "Full connection string", cs => connectionString = cs},
                { "h|help",  "show this message and exit", v => show_help = v != null },
                {"mark", "Mark scripts as executed but take no action", m => mark = true},
            };

            optionSet.Parse(args);

            if (args.Length == 0)
                show_help = true;


            if (show_help)
            {
                optionSet.WriteOptionDescriptions(System.Console.Out);
                return;

            }

            if (String.IsNullOrEmpty(connectionString))
            {
                connectionString = BuildConnectionString(server, database, username, password);
            }

            var dbup = DeployChanges.To
                .SqlDatabase(connectionString)
                .LogToConsole()
                .WithScriptsFromFileSystem(directory)
                .Build();

            if (!mark)
            {
                dbup.PerformUpgrade();
            }
            else
            {
                dbup.MarkAsExecuted();
            }
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
