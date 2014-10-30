using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using MySql.Data.MySqlClient;

namespace DbUp.MySql
{
    /// <summary>
    /// Manages MySql database connections.
    /// </summary>
    public class MySqlConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages MySql database connections.
        /// </summary>
        /// <param name="connectionString"></param>
        public MySqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new MySql database connection.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Splits the statements in the script using the ";" character
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <returns></returns>
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
