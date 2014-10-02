using DbUp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Postgresql;
using DbUp.Engine.Transactions;

public static class PostgresqlExtensions
{
    public static UpgradeEngineBuilder PostgresqlDatabase(this SupportedDatabases supported, string connectionString)
    {
        return PostgresqlDatabase(new PostgresqlConnectionManager(connectionString));
    }

    public static UpgradeEngineBuilder PostgresqlDatabase(IConnectionManager connectionManager)
    {
        throw new NotImplementedException();
        //var builder = new UpgradeEngineBuilder();
        //builder.Configure(c => c.ConnectionManager = connectionManager);
        //builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        //builder.Configure(c => c.Journal = new SqlTableJournal(() => connectionManager, () => c.Log, null, "SchemaVersions"));
        //builder.WithPreprocessor(new SqlCePreprocessor());
        //return builder;
    }
}
