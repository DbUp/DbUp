using DbUp.Engine.Transactions;
using DbUp.Support;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DbUp.SqlServer
{
    /// <summary>
    /// Manages Sql Database Connections
    /// </summary>
    public class SqlConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Manages Sql Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlConnectionManager(string connectionString)
            : base(new DelegateConnectionFactory((log, dbManager) =>
            {
                var conn = new SqlConnection(connectionString);

                if (dbManager.IsScriptOutputLogged)
                    conn.InfoMessage += (sender, e) => log.WriteInformation($"{{0}}{Environment.NewLine}", e.Message);

                return conn;
            }))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}