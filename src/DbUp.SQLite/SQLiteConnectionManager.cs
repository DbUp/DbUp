using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Transactions;

namespace DbUp.SQLite
{
    public class SQLiteConnectionManager : DatabaseConnectionManager
    {
        public SQLiteConnectionManager(string connectionString) : base(connectionString)
        {
        }

        public SQLiteConnectionManager(SQLiteConnection connection) : base(connection)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
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