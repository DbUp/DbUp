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
    public class SqlConnectionManager : DatabaseConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages Sql Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlConnectionManager(string connectionString)
        {
            this.connectionString = connectionString;
            this._sqlContainer = new SqlServerStatementsContainer();
        }
        
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            var conn = new SqlConnection(connectionString);

            if(this.IsScriptOutputLogged)
                conn.InfoMessage += (sender, e) => log.WriteInformation(e.Message + "\r\n");

            return conn;
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}