using DbUp.Builder;
using System;
using DbUp.SqlAnywhere;
using DbUp.Support.SqlAnywhere;
using DbUp.Support.SqlServer;

/// <summary>
/// Configuration extension methods for Sql Anywhere.
/// </summary>
// ReSharper disable once CheckNamespace
public static class SqlAnywhereExtensions
{
    /// <summary>
    /// Creates an upgrader for Sql Anywhere databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Sql Anywhere database connection string.</param>
    /// <param name="journalSchema">The schema under which the journal table will be created</param>
    /// <returns>
    /// A builder for a database upgrader designed for Sql Anywhere databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlAnywhereDatabase(this SupportedDatabases supported, string connectionString, string journalSchema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = new SqlAnywhereConnectionManager(connectionString));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new SqlAnywhereTableJournal(() => c.ConnectionManager, 
                                                                       () => c.Log, 
                                                                       journalSchema, 
                                                                       "schemaversions",
                                                                       new SqlAnywhereSqlPreprocessor()));
        builder.Configure(c => c.ScriptPreprocessors.Add(new SqlAnywhereSqlPreprocessor()));
        return builder;
    }

}