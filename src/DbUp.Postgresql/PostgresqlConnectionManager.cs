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
    public class PostgresqlConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        public PostgresqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new NpgsqlConnection(connectionString);
        }

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
