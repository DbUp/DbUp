using System;
using System.Data;
using System.Data.Common;
using DbUp.Builder;
using DbUp.Support.Oracle;

/// <summary>
/// Configuration extension methods for Oracle.
/// </summary>
/// <remarks>
/// NOTE: DO NOT MOVE THIS TO A NAMESPACE
/// Since the class just contains extension methods, we leave it in the root so that it is always discovered
/// and people don't have to manually add using statements.
/// </remarks>

// ReSharper disable CheckNamespace
public static class OracleExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Default database provider invariant name
    /// </summary>
    public const string DefaultProviderInvariantName = "Oracle.DataAccess";

    /// <summary>
    /// Creates an upgrader for Oracle databases using the default "Oracle.DataAccess" invariant provider name,
    /// and default journal table schema, DBUP.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>
    /// A builder for an Oracle database upgrader
    /// </returns>
    public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString)
    {
        return OracleDatabase(DbProviderFactories.GetFactory(DefaultProviderInvariantName), connectionString, OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName);
    }

    /// <summary>
    /// Creates an upgrader for Oracle databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="providerInvariantName">Invariant name of provider</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>
    /// A builder for an Oracle database upgrader
    /// </returns>
    public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string providerInvariantName, string connectionString)
    {
        return OracleDatabase(DbProviderFactories.GetFactory(providerInvariantName), connectionString, OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName);
    }

    /// <summary>
    /// Creates an upgrader for Oracle databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="providerInvariantName">Invariant name of provider</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="journalTableSchema">Schema in which to create the journal table</param>
    /// <param name="journalTableName"></param>
    /// <returns>
    /// A builder for an Oracle database upgrader
    /// </returns>
    public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string providerInvariantName, string connectionString, string journalTableSchema, string journalTableName)
    {
        return OracleDatabase(DbProviderFactories.GetFactory(providerInvariantName), connectionString, journalTableSchema, journalTableName);
    }

    private static UpgradeEngineBuilder OracleDatabase(DbProviderFactory dbProviderFactory, string connectionString, string journalTableSchema, string journalTableName)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionFactory = () => CreateConnectionFactory(dbProviderFactory, connectionString));
        builder.Configure(c => c.ScriptExecutor = new OracleScriptExecutor(c.ConnectionFactory, () => c.Log, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new OracleTableJournal(c.ConnectionFactory, journalTableSchema, journalTableName, c.Log));
        return builder;
    }

    private static IDbConnection CreateConnectionFactory(DbProviderFactory dbProviderFactory, string connectionString)
    {
        var connection = dbProviderFactory.CreateConnection();
        connection.ConnectionString = connectionString;
        return connection;
    }
}