using System;
using System.Data;
using DbUp.Builder;
using DbUp.Engine.Transactions;
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
        return SqlDatabase(new SqlConnectionManager(connectionString), schema);
    }

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionFactory">The connection factory.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    [Obsolete("Pass connection string instead, then use .WithTransaction() and .WithTransactionPerScript() to manage connection behaviour")]
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
    [Obsolete("Pass connection string instead, then use .WithTransaction() and .WithTransactionPerScript() to manage connection behaviour")]
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, Func<IDbConnection> connectionFactory, string schema)
    {
        return SqlDatabase(new LegacySqlConnectionManager(connectionFactory), schema);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionManager"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    private static UpgradeEngineBuilder SqlDatabase(IConnectionManager connectionManager, string schema)
    {
        return SqlDatabase(connectionManager, schema, "SchemaVersions");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionManager"></param>
    /// <param name="schema"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    private static UpgradeEngineBuilder SqlDatabase(IConnectionManager connectionManager, string schema = null, string table = "SchemaVersions") {
        var builder = new UpgradeEngineBuilder();
        connectionManager.SqlContainer.TableName = table;
        connectionManager.SqlContainer.Scheme = schema;
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log,() => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new TableJournal(() => c.ConnectionManager, () => c.Log));
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
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new TableJournal(()=>c.ConnectionManager, ()=>c.Log));
        return builder;
    }
}
