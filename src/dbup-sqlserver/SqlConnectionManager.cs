using System.Collections.Generic;
using System.Data.SqlClient;
using DbUp.Engine.Transactions;
using DbUp.Support;

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
             : this(new SqlConnection(connectionString))
        { }

        public SqlConnectionManager(SqlConnection connection)
               : base(new DelegateConnectionFactory((log, dbManager) =>
               {
                   if (dbManager.IsScriptOutputLogged)
                       connection.InfoMessage += (sender, e) => log.WriteInformation($"{{0}}", e.Message);

                   return connection;
               }))
        { }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
