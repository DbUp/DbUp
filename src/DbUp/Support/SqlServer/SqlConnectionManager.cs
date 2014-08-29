using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// Manages Sql Database Connections
    /// </summary>
    internal class SqlConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages Sql Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Create connection for Microsoft Sql Server
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            var conn = new SqlConnection(connectionString);

            if(this.IsScriptOutputLogged)
                conn.InfoMessage += (sender, e) => log.WriteInformation(e.Message + "\r\n");

            return conn;
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