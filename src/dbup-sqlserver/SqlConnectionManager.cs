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
        public SqlConnectionManager(string connectionString, bool useAzureSqlIntegratedSecurity)
            : base(new DelegateConnectionFactory((log, dbManager) =>
            {
                var conn = new SqlConnection(connectionString);

#if SUPPORTS_AZURE_AD
                if(useAzureSqlIntegratedSecurity)
                    conn.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").GetAwaiter().GetResult();
#else
                if(useAzureSqlIntegratedSecurity)
                    throw new Exception("You are targeting a framework that does not support Azure AppAuthentication. The minimum target frameworks are .NET Framework 4.5.2 and .NET Standard 1.4.");
#endif

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
