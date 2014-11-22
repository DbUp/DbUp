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
        /// Creates a new MySql database connection.
        /// </summary>
        /// <param name="connectionString">The MySql connection string.</param>
        public MySqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new MySql database connection.
        /// </summary>
        /// <param name="log">The upgrade log.</param>
        /// <returns>The database connection.</returns>
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Splits the statements in the script using the ";" character.
        /// </summary>
        /// <param name="scriptContents">The contents of the script to split.</param>
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
