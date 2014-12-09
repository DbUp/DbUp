using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Transactions;
using MySql.Data.MySqlClient;

namespace DbUp.MySql
{
    /// <summary>
    /// Manages MySql database connections.
    /// </summary>
    public class MySqlConnectionManager : DatabaseConnectionManager
    {
        private readonly Regex regex = new Regex(@"DELIMITER (.+)\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        /// <summary>
        /// Creates a new MySql database connection.
        /// </summary>
        /// <param name="connectionString">The MySql connection string.</param>
        public MySqlConnectionManager(string connectionString) : base(new DelegateConnectionFactory(l => new MySqlConnection(connectionString)))
        {
        }

        /// <summary>
        /// Splits the statements in the script using the ";" character or 
        /// DELIMITER if specified.
        /// </summary>
        /// <param name="scriptContents">The contents of the script to split.</param>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var delimiterMatch = regex.Match(scriptContents);

            if (!delimiterMatch.Success)
            {
                return scriptContents.Split(';')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0).ToList();
            }
            else
            {
                return GetDelimitedCommands(scriptContents);
            }
        }

        private IEnumerable<string> GetDelimitedCommands(string scriptContents)
        {
            var delimiterMatch = regex.Match(scriptContents);

            var nonDelimiterScriptContents = regex.Split(scriptContents)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            var standardDelimiterString = nonDelimiterScriptContents[0];

            var delimiter = delimiterMatch.Groups[1].Value;

            var newDelimiterString = nonDelimiterScriptContents[2];

            var scriptStatements =
                standardDelimiterString.Split(';')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0).ToList();

                scriptStatements.AddRange(newDelimiterString.Split(delimiter.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0));

            return scriptStatements;

        }
    }
}
