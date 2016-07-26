using DbUp.Builder;
using System;
using DbUp.SqlAnywhere;
using DbUp.Support.SqlServer;
using DbUp.Support.MySql;

/// <summary>
/// Configuration extension methods for MySql.
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
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new MySqlITableJournal(() => c.ConnectionManager, () => c.Log, null, "schemaversions"));
        return builder;
    }

}