using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.Support.AzureSqlDataWarehouse;
using DbUp.Support.SqlServer;

/// <summary>
/// Configuration extension methods for Azure SQL Data Warehouse
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class AzureSqlDataWarehouseExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for Azure SQL Data Warehouse databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Azure SQL Data Warehouse databases.
    /// </returns>
    public static UpgradeEngineBuilder AzureSqlDataWarehouseDatabase(this SupportedDatabases supported, string connectionString)
    {
        return AzureSqlDataWarehouseDatabase(new SqlConnectionManager(connectionString));
    }

    private static UpgradeEngineBuilder AzureSqlDataWarehouseDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new AzureSqlDataWarehouseTableJournal(() => connectionManager, () => c.Log, "SchemaVersions"));
        return builder;
    }
}
