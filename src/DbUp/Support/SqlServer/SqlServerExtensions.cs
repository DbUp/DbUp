using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using DbUp;
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
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors));
        builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, "SchemaVersions"));
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
        builder.Configure(c => c.Journal = new SqlTableJournal(()=>c.ConnectionManager, ()=>c.Log, schema, table));
        return builder;
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim() == string.Empty)
        {
            throw new ArgumentNullException("connectionString");
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        var databaseName = connectionStringBuilder.InitialCatalog;

        if (string.IsNullOrEmpty(databaseName) || databaseName.Trim() == string.Empty)
        {
            throw new InvalidOperationException("The connection string does not specify a database name.");
        }

        connectionStringBuilder.InitialCatalog = "master";

        // TODO: Where to log output from this method? There is no Log context at this point.
        Debug.WriteLine("Master ConnectionString => {0}", connectionStringBuilder.ToString());

        using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();
            
            var sqlCommandText = string.Format
                (
                    @"select case when db_id('{0}') is not null then 1 else 0 end;",
                    databaseName
                );


            // check to see if the database already exists..
            using (var command = new SqlCommand(sqlCommandText, connection)
            {
                CommandType = CommandType.Text
            })
            {
                var results = (int) command.ExecuteScalar();

                // if the database exists, we're done here...
                if (results == 1) return;
            }

            sqlCommandText = string.Format
                    (
                        @"create database [{0}];",
                        databaseName
                    );

            // Create the database...
            using (var command = new SqlCommand(sqlCommandText, connection)
            {
                CommandType = CommandType.Text
            })
            {
                command.ExecuteNonQuery();

            }

            // TODO: Where to log output from this method? There is no Log context at this point.
            Console.WriteLine(@"Created database {0}", databaseName);
        }
    }


}
