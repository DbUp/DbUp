using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DbUp.Postgresql
{
    /// <summary>
    /// Manages PostgreSQL database connections.
    /// </summary>
    public class PostgresqlConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Creates a new PostgreSQL database connection.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        public PostgresqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new PostgreSQL database connection.
        /// </summary>
        /// <param name="log">The upgrade log.</param>
        /// <returns>The database connection.</returns>
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new NpgsqlConnection(connectionString);
        }

		protected override IDbConnection CreateSystemConnection()
		{
			var parameters = connectionString.Split(';');
			var sysConnectionString = parameters.Where(s => !s.StartsWith("Database", StringComparison.InvariantCultureIgnoreCase))
				.Aggregate(new StringBuilder(), (sb, s) => (sb.Length != 0) ? sb.Append(";" + s) : sb.Append(s))
				.ToString();

			return new NpgsqlConnection(sysConnectionString);
		}

        /// <summary>
        /// Get the system connection string and then try to execute
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override bool EnsureDatabase(string name)
        {
            bool result = false;

            using (var conn = CreateSystemConnection() as NpgsqlConnection)
            {
                int? scalarResult = null;
                conn.Open();
                using (var cmd = new Npgsql.NpgsqlCommand($"select 1 from pg_database where datname = \"{name}\"", conn))
                {
                    scalarResult = (int?)cmd.ExecuteScalar();
                }

                if (scalarResult.HasValue && scalarResult.Value == 1)
                {
                    result = true;
                }
                else
                {
                    using (var cmd = new NpgsqlCommand($"create database \"{name}\""))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            // log, rethrow, or something else
                        }
                    }
                }

            }

            return result;
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
