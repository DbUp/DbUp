using System;
using DbUp.Builder;
using DbUp.Support.SQLite;
using DbUp.SQLite;
using DbUp.Support.SqlServer;
using System.Data.SQLite;

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
    /// Creates an upgrader for SQLite databases with a given connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">SQLite database connection string</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, string connectionString)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = new SQLiteConnectionManager(connectionString));
        builder.Configure(c => c.Journal = new SQLiteTableJournal(() => c.ConnectionManager, () => c.Log, "SchemaVersions"));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null,
            () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.WithPreprocessor(new SQLitePreprocessor());
        return builder;
    }

    /// <summary>
    /// Creates an upgrade for SQLite databases with a given database connection 
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="sqliteConnection">Given SQLite database connection object which is used by DbUp upgrade engine</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, SQLiteConnection sqliteConnection)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = new SQLiteConnectionManager(sqliteConnection));
        builder.Configure(c => c.Journal = new SQLiteTableJournal(() => c.ConnectionManager, () => c.Log, "SchemaVersions"));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null,
            () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.WithPreprocessor(new SQLitePreprocessor());
        return builder;
    }
}