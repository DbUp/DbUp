using System;
using System.Data;
using System.Data.SqlClient;
using DbUp;
using DbUp.AzureSqlDataWarehouse;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

/// <summary>
/// Configuration extension methods for Azure SQL Data Warehouse.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the global:: namespace so that it is always available
// ReSharper disable CheckNamespace
public static class AzureSqlDwServerExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for Azure SQL Data Warehouse databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Azure SQL Data Warehouse databases.
    /// </returns>
    public static UpgradeEngineBuilder AzureSqlDataWarehouse(this SupportedDatabases supported, string connectionString)
    {
        return AzureSqlDataWarehouse(supported, connectionString, null);
    }

    /// <summary>
    /// Creates an upgrader for Azure SQL Data Warehouse databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Azure SQL Data Warehouse databases.
    /// </returns>
    public static UpgradeEngineBuilder AzureSqlDataWarehouse(this SupportedDatabases supported, string connectionString, string schema)
    {
        return AzureSqlDataWarehouse(new AzureSqlDwConnectionManager(connectionString), schema);
    }

    /// <summary>
    /// Creates an upgrader for Azure SQL Data Warehouse databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SqlConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Azure SQL Data Warehouse databases.
    /// </returns>
    public static UpgradeEngineBuilder AzureSqlDataWarehouse(this SupportedDatabases supported, IConnectionManager connectionManager, string schema = null)
        => AzureSqlDataWarehouse(connectionManager, schema);

    /// <summary>
    /// Creates an upgrader for Azure SQL Data Warehouse databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SqlConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Azure SQL Data Warehouse databases.
    /// </returns>
    private static UpgradeEngineBuilder AzureSqlDataWarehouse(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new AzureSqlDwScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new AzureSqlDwTableJournal(() => c.ConnectionManager, () => c.Log, schema, "SchemaVersions"));
        return builder;
    }

    /// <summary>
    /// Tracks the list of executed scripts in an Azure SQL Data Warehouse table.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder JournalToAzureSqlDwTable(this UpgradeEngineBuilder builder, string schema, string table)
    {
        builder.Configure(c => c.Journal = new AzureSqlDwTableJournal(() => c.ConnectionManager, () => c.Log, schema, table));
        return builder;
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void AzureSqlDataWarehouse(this SupportedDatabasesForEnsureDatabase supported, string connectionString)
    {
        AzureSqlDataWarehouse(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void AzureSqlDataWarehouse(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout)
    {
        AzureSqlDataWarehouse(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void AzureSqlDataWarehouse(this SupportedDatabasesForEnsureDatabase supported, string connectionString, IUpgradeLog logger, int timeout = -1)
    {
        string databaseName;
        string masterConnectionString;
        GetMasterConnectionStringBuilder(connectionString, logger, out masterConnectionString, out databaseName);

        using (var connection = new SqlConnection(masterConnectionString))
        {
            connection.Open();

            var sqlCommandText = string.Format
                (
                    @"SELECT TOP 1 case WHEN dbid IS NOT NULL THEN 1 ELSE 0 end FROM sys.sysdatabases WHERE name = '{0}';",
                    databaseName
                );


            // check to see if the database already exists..
            using (var command = new SqlCommand(sqlCommandText, connection)
            {
                CommandType = CommandType.Text
            })

            {
                var results = (int?)command.ExecuteScalar();

                // if the database exists, we're done here...
                if (results.HasValue && results.Value == 1)
                {
                    return;
                }
            }

            var parser = new AzureSqlDwServerObjectParser();
            databaseName = parser.QuoteIdentifier(databaseName);

            sqlCommandText = $"CREATE DATABASE {databaseName} (EDITION = 'DataWarehouse', SERVICE_OBJECTIVE = 'DW100', MAXSIZE = 250 GB);";

            // Create the database...
            using (var command = new SqlCommand(sqlCommandText, connection)
            {
                CommandType = CommandType.Text
            })
            {
                if (timeout >= 0)
                {
                    command.CommandTimeout = timeout;
                }

                command.ExecuteNonQuery();

            }

            logger.WriteInformation(@"Created database {0}", databaseName);
        }
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void AzureSqlDataWarehouse(this SupportedDatabasesForDropDatabase supported, string connectionString)
    {
        AzureSqlDataWarehouse(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void AzureSqlDataWarehouse(this SupportedDatabasesForDropDatabase supported, string connectionString, int commandTimeout)
    {
        AzureSqlDataWarehouse(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void AzureSqlDataWarehouse(this SupportedDatabasesForDropDatabase supported, string connectionString, IUpgradeLog logger, int timeout = -1)
    {
        string databaseName;
        string masterConnectionString;
        GetMasterConnectionStringBuilder(connectionString, logger, out masterConnectionString, out databaseName);

        using (var connection = new SqlConnection(masterConnectionString))
        {
            connection.Open();
            var databaseExistCommand = new SqlCommand($"SELECT TOP 1 case WHEN dbid IS NOT NULL THEN 1 ELSE 0 end FROM sys.sysdatabases WHERE name = '{databaseName}';", connection)
            {
                CommandType = CommandType.Text
            };
            using (var command = databaseExistCommand)
            {
                var exists = (int?)command.ExecuteScalar();
                if (!exists.HasValue)
                    return;
            }

            var parser = new AzureSqlDwServerObjectParser();
            databaseName = parser.QuoteIdentifier(databaseName);

            var dropDatabaseCommand = new SqlCommand($"ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {databaseName};", connection) { CommandType = CommandType.Text };
            using (var command = dropDatabaseCommand)
            {
                command.ExecuteNonQuery();
            }

            logger.WriteInformation("Dropped database {0}", databaseName);
        }
    }

    private static void GetMasterConnectionStringBuilder(string connectionString, IUpgradeLog logger, out string masterConnectionString, out string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim() == string.Empty)
            throw new ArgumentNullException("connectionString");

        if (logger == null)
            throw new ArgumentNullException("logger");

        var masterConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        databaseName = masterConnectionStringBuilder.InitialCatalog;

        if (string.IsNullOrEmpty(databaseName) || databaseName.Trim() == string.Empty)
            throw new InvalidOperationException("The connection string does not specify a database name.");

        masterConnectionStringBuilder.InitialCatalog = "master";
        var logMasterConnectionStringBuilder = new SqlConnectionStringBuilder(masterConnectionStringBuilder.ConnectionString)
        {
            Password = string.Empty.PadRight(masterConnectionStringBuilder.Password.Length, '*')
        };

        logger.WriteInformation("Master ConnectionString => {0}", logMasterConnectionStringBuilder.ConnectionString);
        masterConnectionString = masterConnectionStringBuilder.ConnectionString;
    }
}
