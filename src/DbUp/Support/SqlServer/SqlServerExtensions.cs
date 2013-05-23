using System;
using System.Data;
using System.Data.SqlClient;
using DbUp.Builder;
using DbUp.Support.SqlServer;

/// <summary>
/// Configuration extension methods for SQL Server.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the root so that it is always discovered
// and people don't have to manually add using statements.
// ReSharper disable CheckNamespace
public static class SqlServerExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, string connectionString)
    {
        return SqlDatabase(supported, connectionString, null);
    }

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, string connectionString, string schema)
    {
        return SqlDatabase(supported, () => new SqlConnection(connectionString), schema);
    }

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionFactory">The connection factory.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, Func<IDbConnection> connectionFactory)
    {
        return SqlDatabase(supported, connectionFactory, null);
    }

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionFactory">The connection factory.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, Func<IDbConnection> connectionFactory, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionFactory = connectionFactory);
        builder.Configure(c => c.SqlScriptExecutor = new SqlScriptExecutor(c.ConnectionFactory, schema));
        builder.Configure(c => c.Journal = new SqlTableJournal(c.ConnectionFactory, schema, "SchemaVersions", c.Log));
        return builder;
    }

    /// <summary>
    /// Tracks the list of executed scripts in a SQL Server table.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder JournalToSqlTable(this UpgradeEngineBuilder builder, string schema, string table)
    {
        builder.Configure(c => c.Journal = new SqlTableJournal(c.ConnectionFactory, schema, table, c.Log));
        return builder;
    }
}
