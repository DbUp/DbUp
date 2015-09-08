using DbUp.Builder;
using System;
using DbUp.Firebird;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;
using DbUp.Support.Firebird;

// ReSharper disable once CheckNamespace

/// <summary>
/// Configuration extension methods for Firebird.
/// </summary>
public static class FirebirdExtensions
{
    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Firebird database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(this SupportedDatabases supported, string connectionString)
    {
        return FirebirdDatabase(new FirebirdConnectionManager(connectionString));
    }

    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="FirebirdConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new FirebirdTableJournal(() => c.ConnectionManager, () => c.Log, "schemaversions"));
        builder.WithPreprocessor(new FirebirdPreprocessor());
        return builder;
    }
}
