using System.Collections.Generic;

#if SUPPORTS_MICROSOFT_SQL_CLIENT
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

using DbUp.Engine.Transactions;
using DbUp.Support;

#if SUPPORTS_AZURE_AD
using Azure.Identity;
using Azure.Core;

namespace DbUp.SqlServer
{
    /// <summary>Manages an Azure Sql Server database connection.</summary>
    public class AzureSqlConnectionManager : DatabaseConnectionManager
    {
        public AzureSqlConnectionManager(string connectionString)
            : this(connectionString, "https://database.windows.net/", null)
        { }

        public AzureSqlConnectionManager(string connectionString, string resource)
            : this(connectionString, resource, null)
        { }

        public AzureSqlConnectionManager(string connectionString, string resource, string tenantId, string azureAdInstance = "https://login.microsoftonline.com/")
            : base(new DelegateConnectionFactory((log, dbManager) =>
            {
                var tokenProvider = new DefaultAzureCredential();
                var tokenContext = new TokenRequestContext(scopes: new string[] { resource + "/.default" });
                var conn = new SqlConnection(connectionString)
                {
                    AccessToken = tokenProvider.GetTokenAsync(tokenContext)
                                               .ConfigureAwait(false)
                                               .GetAwaiter()
                                               .GetResult()
                                               .Token
                };

                if (dbManager.IsScriptOutputLogged)
                    conn.InfoMessage += (sender, e) => log.WriteInformation($"{{0}}", e.Message);

                return conn;
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
#endif
