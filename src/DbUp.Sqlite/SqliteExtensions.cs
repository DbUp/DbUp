using System;
using DbUp.Builder;
using DbUp.SQLite.Engine;
using DbUp.SQLite.Helpers;
using DbUp.Support.SqlServer;

/// <summary>
/// Configuration extension methods for SQLite (see http://www.sqlite.org/)
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class SQLiteExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for SQLite databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">SQLite database connection string</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, string connectionString)
    {
        return SQLiteDatabase(supported, connectionString, "SchemaVersions");
    }

    /// <summary>
    /// Creates an upgrader for SQLite databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">SQLite database connection string</param>
    /// <param name="tableName">Name of journaling table.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, string connectionString, string tableName) {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = new ConnectionManager(connectionString));
        builder.Configure(c => c.QueryProvider = new QueryProvider(tableName));
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log, () => c.QueryProvider));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, () => c.QueryProvider,
            () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.WithPreprocessor(new ScriptPreprocessor());
        return builder;
    }

    /// <summary>
    /// Creates an upgrader for SQLite databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="sharedConnection">SQLite database connection which you control when it is closed</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, SharedConnection sharedConnection)
    {
        return SQLiteDatabase(supported, sharedConnection, "SchemaVersions");
    }

    /// <summary>
    /// Creates an upgrader for SQLite databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="sharedConnection">SQLite database connection which you control when it is closed</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, SharedConnection sharedConnection, string tableName) {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = new ConnectionManager(sharedConnection));
        builder.Configure(c => c.QueryProvider = new QueryProvider(tableName));
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log, () => c.QueryProvider));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, () => c.QueryProvider,
            () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.WithPreprocessor(new ScriptPreprocessor());
        return builder;
    }
}