using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SQLite.Helpers;

namespace DbUp.SQLite
{
    /// <summary>
    /// SQLite Connection Manager.
    /// </summary>
    internal class ConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;
        private readonly SharedConnection sharedConnection;

        public ConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ConnectionManager(SharedConnection sharedConnection)
        {
            this.sharedConnection = sharedConnection;
        }

        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            // if we have a shared connection, return it, otherwise create a connection
            return (IDbConnection)sharedConnection ?? new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// Sqlite statements seprator is ; (see http://www.sqlite.org/lang.html)
        /// </summary>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var scriptStatements =
                Regex.Split(scriptContents, "^\\s*;\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            return scriptStatements;
        }
    }
}