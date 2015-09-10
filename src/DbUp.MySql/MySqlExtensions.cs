using DbUp.Builder;
using System;
using DbUp;
using DbUp.MySql;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;
using DbUp.Support.MySql;

/// <summary>
/// Configuration extension methods for MySql.
/// </summary>
// ReSharper disable once CheckNamespace
public static class MySqlExtensions
{
    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">MySql database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(this SupportedDatabases supported, string connectionString)
    {
        return MySqlDatabase(new MySqlConnectionManager(connectionString));
    }

    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="MySqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new MySqlITableJournal(() => c.ConnectionManager, () => c.Log, null, "schemaversions"));
        builder.WithPreprocessor(new MySqlPreprocessor());
        return builder;
    }
}