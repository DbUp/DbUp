using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.SqlCe
{
    /// <summary>
    /// Manages SqlCe Database Connections
    /// </summary>
    internal class ConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages SqlCe Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public ConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Create connection for Sql CE.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new SqlCeConnection(connectionString);
        }
        /// <summary>
        /// Split Scripts into statements.
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <returns></returns>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var scriptStatements =
            Regex.Split(scriptContents, "^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            return scriptStatements;
        }
    }
}