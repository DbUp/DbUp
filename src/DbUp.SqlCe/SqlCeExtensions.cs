using System;
using System.Data.SqlServerCe;
using DbUp.Builder;
using DbUp.SqlCe;
using DbUp.Support.SqlServer;

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
    public static UpgradeEngineBuilder SqlCeDatabase(this SupportedDatabases supported, Func<SqlCeConnection> connectionFactory)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionFactory = connectionFactory);
        builder.Configure(c => c.SqlScriptExecutor = new SqlScriptExecutor(c.ConnectionFactory,  null));
        builder.Configure(c => c.Journal = new SqlTableJournal(c.ConnectionFactory, null, "SchemaVersions", c.Log));
        builder.WithPreprocessor(new SqlCePreprocessor());
        return builder;
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
        return supported.SqlCeDatabase(() => new SqlCeConnection(connectionString));
    }
}
