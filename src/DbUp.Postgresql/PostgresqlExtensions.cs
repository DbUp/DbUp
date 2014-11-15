using DbUp.Builder;
using System;
using DbUp.Postgresql;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;

// ReSharper disable once CheckNamespace

/// <summary>
/// Configuration extension methods for PostgreSQL.
/// </summary>
public static class PostgresqlExtensions
{
    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">PostgreSQL database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(this SupportedDatabases supported, string connectionString)
    {
        return PostgresqlDatabase(new PostgresqlConnectionManager(connectionString));
    }

    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="PostgresqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ConnectionManager.SetSqlContainerParameters("schemaversions", null));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, () => false, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log));
        builder.WithPreprocessor(new PostgresqlPreprocessor());
        return builder;
    }
}
