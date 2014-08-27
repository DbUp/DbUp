using System;
using System.Collections.Generic;
using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.Oracle;
using DbUp.Oracle.Devart;
using DbUp.Oracle.ODPnet;

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
    private static string selectedProvider = "Devart";

    private static readonly Dictionary<string, DatabaseConnectionManager> providerSimpleFactory = new Dictionary<string, DatabaseConnectionManager>()
    {
        {"Devart", new DevartOracleConnectionManager()},
        {"Oracle", new OracleConnectionManager()}
    };

    /// <summary>
    /// Creates an upgrader for Oracle databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>
    /// A builder for an Oracle database upgrader
    /// </returns>
    public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString)
    {
        DatabaseConnectionManager manager = providerSimpleFactory[selectedProvider];
        manager.ConnectionString = connectionString;
        return OracleDatabase(manager);
    }

    private static UpgradeEngineBuilder OracleDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new ScriptExecutor(() => c.ConnectionManager, () => c.Log, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log));
        return builder;
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
        selectedProvider = providerInvariantName;
        return OracleDatabase(supported, connectionString);
    }
}

