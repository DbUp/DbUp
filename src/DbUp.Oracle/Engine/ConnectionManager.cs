using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using Oracle.ManagedDataAccess.Client;

namespace DbUp.Oracle.Engine
{
    /// <summary>
    /// Default oracle connection manager based on Oracle ODP.NET
    /// </summary>
    internal class ConnectionManager : DatabaseConnectionManager
    {
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            // if we have a shared connection, return it, otherwise create a connection
            return new OracleConnection(ConnectionString);
        }

        /// <summary>
        /// Oracle statements seprator is /
        /// </summary>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
             var scriptStatements =
                Regex.Split(scriptContents, "/\r*$", RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            return scriptStatements;
        }
    }
}
