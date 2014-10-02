using DbUp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Postgresql;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;
using DbUp.Support.Postgresql;

public static class PostgresqlExtensions
{
    public static UpgradeEngineBuilder PostgresqlDatabase(this SupportedDatabases supported, string connectionString)
    {
        return PostgresqlDatabase(new PostgresqlConnectionManager(connectionString));
    }

    public static UpgradeEngineBuilder PostgresqlDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new PostgresqlTableJournal(() => c.ConnectionManager, () => c.Log, null, "SchemaVersions"));
        builder.WithPreprocessor(new PostgresqlPreprocessor());
        return builder;
    }
}
