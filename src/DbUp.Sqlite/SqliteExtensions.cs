using System;
using System.Data;
using System.Data.SQLite;
using DbUp.Builder;
using DbUp.Support.Sqlite;
using DbUp.Sqlite;

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
    /// <param name="connectionFactory">The SQLite connection factory.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQLite databases.
    /// </returns>
    public static UpgradeEngineBuilder SQLiteDatabase(this SupportedDatabases supported, Func<IDbConnection> connectionFactory)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionFactory = connectionFactory);
        builder.Configure(c => c.ScriptExecutor = new SQLiteScriptExecutor(c.ConnectionFactory, () => c.Log, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new SQLiteTableJournal(c.ConnectionFactory, "SchemaVersions", c.Log));
        builder.WithPreprocessor(new SQLitePreprocessor());
        return builder;
    }

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
        return supported.SQLiteDatabase(() => new SQLiteConnection(connectionString));
    }
}

