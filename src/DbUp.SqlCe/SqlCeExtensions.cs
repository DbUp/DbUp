using System;
using System.Data.SqlServerCe;
using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.SqlCe.Engine;
using DbUp.Support.SqlServer;
using DbUp.SqlCe;

/// <summary>
/// Configuration extension methods for SQL CE.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class SqlCeExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for SQL CE databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionFactory">The connection factory.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    [Obsolete("Pass connection string instead, then use .WithTransaction() and .WithTransactionPerScript() to manage connection behaviour")]
    public static UpgradeEngineBuilder SqlCeDatabase(this SupportedDatabases supported, Func<SqlCeConnection> connectionFactory)
    {
        return SqlCeDatabase(new LegacySqlConnectionManager(connectionFactory));        
    }

    /// <summary>
    /// Creates an upgrader for SQL CE databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlCeDatabase(this SupportedDatabases supported, string connectionString)
    {
        return SqlCeDatabase(new SqlCeConnectionManager(connectionString));
    }

    private static UpgradeEngineBuilder SqlCeDatabase(IConnectionManager connectionManager, string table = "SchemaVersions")
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.SqlStatementsContainer = new SqlCeStatements(table));
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, () => c.SqlStatementsContainer, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new TableJournal(() => connectionManager, () => c.Log, () => c.SqlStatementsContainer));
        builder.WithPreprocessor(new SqlCePreprocessor());
        return builder;
    }
}
