using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.SqlCe;

/// <summary>
/// Configuration extension methods for SQL CE.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class SqlCeExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for SQL CE databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlCeDatabase(this SupportedDatabases supported, string connectionString)
    {
        return SqlCeDatabase(new SqlCeConnectionManager(connectionString));
    }

    /// <summary>
    /// Creates an upgrader for SQL CE databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlCeDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
        => SqlCeDatabase(connectionManager);

    static UpgradeEngineBuilder SqlCeDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlCeScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new SqlCeTableJournal(() => connectionManager, () => c.Log, null, "SchemaVersions"));
        builder.WithPreprocessor(new SqlCePreprocessor());
        return builder;
    }
}
