using System.Collections.Generic;
using System.Data.SqlClient;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SqlServer
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
                    conn.InfoMessage += (sender, e) => log.WriteInformation(e.Message + "\r\n");

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