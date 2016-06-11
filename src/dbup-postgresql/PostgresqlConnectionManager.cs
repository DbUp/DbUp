using DbUp.Engine.Transactions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DbUp.Postgresql
{
    /// <summary>
    /// Manages PostgreSQL database connections.
    /// </summary>
    public class PostgresqlConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Creates a new PostgreSQL database connection.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        public PostgresqlConnectionManager(string connectionString) : base(new DelegateConnectionFactory(l => new NpgsqlConnection(connectionString)))
        {
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
