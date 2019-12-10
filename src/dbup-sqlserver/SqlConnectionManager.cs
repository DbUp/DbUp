using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DbUp.Engine.Transactions;
using DbUp.Support;
#if SUPPORTS_AZURE_AD
using Microsoft.Azure.Services.AppAuthentication;
#endif

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

#if SUPPORTS_AZURE_AD
        /// <summary>
        /// Manages Sql Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="useAzureSqlIntegratedSecurity">Whether to use Azure SQL Integrated Sercurity</param>
        public SqlConnectionManager(string connectionString, bool useAzureSqlIntegratedSecurity)
            : base(new DelegateConnectionFactory((log, dbManager) =>
            {
                var conn = new SqlConnection(connectionString);

                if (useAzureSqlIntegratedSecurity)
                    conn.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").ConfigureAwait(false).GetAwaiter().GetResult();

                if (dbManager.IsScriptOutputLogged)
                    conn.InfoMessage += (sender, e) => log.WriteInformation($"{{0}}{Environment.NewLine}", e.Message);

                return conn;
            }))
        {
        }
#endif

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
