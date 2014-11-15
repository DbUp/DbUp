using System;
using DbUp.Builder;
using DbUp.SQLite;
using DbUp.SQLite.Helpers;
using DbUp.Support.SqlServer;
using DbUp.SQLite.Engine;

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
    /// <param name="journalTableName">Name of journaling table.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, string connectionString, string journalTableName) {
        var builder = new UpgradeEngineBuilder();
        var connectionManager = new SQLiteConnectionManager(connectionString);
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ConnectionManager.SqlContainer.TableName = journalTableName);
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log,
            () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.WithPreprocessor(new SQLitePreprocessor());
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
    /// <param name="journalTableName">Name of journaling table.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, SharedConnection sharedConnection, string journalTableName) {
        var builder = new UpgradeEngineBuilder();
        var connectionManager = new SQLiteConnectionManager(sharedConnection);
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ConnectionManager.SqlContainer.TableName = journalTableName);
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log,
            () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.WithPreprocessor(new SQLitePreprocessor());
        return builder;
    }
}