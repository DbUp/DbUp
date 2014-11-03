using System;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support.MySql;

namespace DbUp.MySql
{
    public static class MySqlExtensions
    {
        public static UpgradeEngineBuilder MySqlDatabase(this SupportedDatabases supported, string connectionString)
        {
            return MySqlDatabase(new MySqlConnectionManager(connectionString));    
        }

        private static UpgradeEngineBuilder MySqlDatabase(IConnectionManager connectionManager)
        {
            var builder = new UpgradeEngineBuilder();
            builder.Configure(c => c.ConnectionManager = connectionManager);
            builder.Configure(c => c.ScriptExecutor = new ScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => false, c.ScriptPreprocessors, new MySqlObjectParser(), new MySqlCreateSchema()));
            builder.Configure(c => c.Journal = new MySqlTableJournal(() => c.ConnectionManager, () => c.Log, null, "schemaversions"));
            
            return builder;
        }
    }
}
