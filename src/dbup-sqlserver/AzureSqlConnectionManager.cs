using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DbUp.Engine.Transactions;
using DbUp.Support;

#if SUPPORTS_AZURE_AD
using Microsoft.Azure.Services.AppAuthentication;

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
                var conn = new SqlConnection(connectionString)
                {
                    AccessToken = new AzureServiceTokenProvider(azureAdInstance: azureAdInstance).GetAccessTokenAsync(resource, tenantId)
                                                                                                 .ConfigureAwait(false)
                                                                                                 .GetAwaiter()
                                                                                                 .GetResult()
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
