using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.SqlAnywhere;

/// <summary>
/// Configuration extension methods for SQL Anywhere.
/// </summary>
// ReSharper disable once CheckNamespace
public static class SqlAnywhereExtensions
{
    /// <summary>
    /// Creates an upgrader for Sql Anywhere databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Sql Anywhere database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Sql Anywhere databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlAnywhereDatabase(this SupportedDatabases supported, string connectionString)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = new SqlAnywhereConnectionManager(connectionString));
        builder.Configure(c => c.ScriptExecutor = new SqlAnywhereScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new SqlAnywhereTableJournal(() => c.ConnectionManager, () => c.Log, null, "schemaversions"));
        return builder;
    }

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SqlAnywhereConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SqlAnywhere schema name to use. Defaults to ''.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SqlAnywhere databases.
    /// </returns>
    static UpgradeEngineBuilder SqlAnywhereDatabase(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlAnywhereScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new SqlAnywhereTableJournal(() => c.ConnectionManager, () => c.Log, schema, "SchemaVersions"));
        return builder;
    }






}