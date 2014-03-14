using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine;
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
            var parser = new SqlBatchParser();

            var scriptStatements = parser.SplitScriptBatches(scriptContents, "GO");
            
            return scriptStatements;
        }
    }
}