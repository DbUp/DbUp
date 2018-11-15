using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.AzureSqlDataWarehouse
{
    /// <summary>
    /// Manages Azure SQL Date Warehouse Database Connections
    /// </summary>
    public class AzureSqlDwConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Manages Azure SQL Date Warehouse Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public AzureSqlDwConnectionManager(string connectionString)
            : base(new DelegateConnectionFactory((log, dbManager) =>
            {
                var conn = new SqlConnection(connectionString);

                if (dbManager.IsScriptOutputLogged)
                    conn.InfoMessage += (sender, e) => log.WriteInformation("{0}\r\n", e.Message);

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