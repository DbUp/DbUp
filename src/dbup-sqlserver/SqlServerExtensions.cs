using System;
using System.Data;
using System.Data.SqlClient;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SqlServer;

/// <summary>
/// Configuration extension methods for SQL Server.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the global:: namespace so that it is always available
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

#if SUPPORTS_AZURE_AD
    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo' if null.</param>
    /// <param name="useAzureSqlIntegratedSecurity">Whether to use Azure SQL Integrated Security</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, string connectionString, string schema, bool useAzureSqlIntegratedSecurity)
    {
        return SqlDatabase(new SqlConnectionManager(connectionString, useAzureSqlIntegratedSecurity), schema);
    }
#endif

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SqlConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SqlDatabase(this SupportedDatabases supported, IConnectionManager connectionManager, string schema = null)
        => SqlDatabase(connectionManager, schema);

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SqlConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    static UpgradeEngineBuilder SqlDatabase(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SqlScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new SqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, "SchemaVersions"));
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
        builder.Configure(c => c.Journal = new SqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, table));
        return builder;
    }

#if SUPPORTS_SQL_CONTEXT
    /// <summary>
    /// Logs to SqlContext.Pipe, for use with "context connection=true".
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The same builder
    /// </returns>
    public static UpgradeEngineBuilder LogToSqlContext(this UpgradeEngineBuilder builder)
    {
        return builder.LogTo(new SqlContextUpgradeLog());
    }
#endif

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="azureDatabaseEdition">Azure edition to Create</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, AzureDatabaseEdition azureDatabaseEdition)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), -1, azureDatabaseEdition);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, string collation)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), collation: collation);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="azureDatabaseEdition">Azure edition to Create</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, AzureDatabaseEdition azureDatabaseEdition)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout, azureDatabaseEdition);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, string collation)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout, collation: collation);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="azureDatabaseEdition">Azure edition to Create</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, AzureDatabaseEdition azureDatabaseEdition, string collation)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), azureDatabaseEdition: azureDatabaseEdition, collation: collation);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="azureDatabaseEdition">Azure edition to Create</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, AzureDatabaseEdition azureDatabaseEdition, string collation)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout, azureDatabaseEdition, collation);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="azureDatabaseEdition">Use to indicate that the SQL server database is in Azure</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SqlDatabase(
        this SupportedDatabasesForEnsureDatabase supported,
        string connectionString,
        IUpgradeLog logger,
        int timeout = -1,
        AzureDatabaseEdition azureDatabaseEdition = AzureDatabaseEdition.None,
        string collation = null)
    {
        GetMasterConnectionStringBuilder(connectionString, logger, out var masterConnectionString, out var databaseName);

        using (var connection = new SqlConnection(masterConnectionString))
        {
            try
            {
                connection.Open();
            }
            catch (SqlException)
            {
                // Failed to connect to master, lets try direct  
                if (DatabaseExistsIfConnectedToDirectly(logger, connectionString, databaseName))
                    return;

                throw;
            }

            if (DatabaseExists(connection, databaseName))
                return;

            var collationString = string.IsNullOrEmpty(collation) ? "" : $@" COLLATE {collation}";
            var sqlCommandText = $@"create database [{databaseName}]{collationString}";

            switch (azureDatabaseEdition)
            {
                case AzureDatabaseEdition.None:
                    sqlCommandText += ";";
                    break;
                case AzureDatabaseEdition.Basic:
                    sqlCommandText += " ( EDITION = ''basic'' );";
                    break;
                case AzureDatabaseEdition.Standard:
                    sqlCommandText += " ( EDITION = ''standard'' );";
                    break;
                case AzureDatabaseEdition.Premium:
                    sqlCommandText += " ( EDITION = ''premium'' );";
                    break;
            }


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

    static bool DatabaseExistsIfConnectedToDirectly(IUpgradeLog logger, string connectionString, string databaseName)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return DatabaseExists(connection, databaseName);
            }
        }
        catch
        {
            logger.WriteInformation("Could not connect to the database directly");
            return false;
        }
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, int commandTimeout)
    {
        SqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void SqlDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, IUpgradeLog logger, int timeout = -1)
    {
        GetMasterConnectionStringBuilder(connectionString, logger, out var masterConnectionString, out var databaseName);

        using (var connection = new SqlConnection(masterConnectionString))
        {
            connection.Open();
            if (!DatabaseExists(connection, databaseName))
                return;

            var dropDatabaseCommand = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}];", connection) { CommandType = CommandType.Text };
            using (var command = dropDatabaseCommand)
            {
                command.ExecuteNonQuery();
            }

            logger.WriteInformation("Dropped database {0}", databaseName);
        }
    }

    static void GetMasterConnectionStringBuilder(string connectionString, IUpgradeLog logger, out string masterConnectionString, out string databaseName)
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

    static bool DatabaseExists(SqlConnection connection, string databaseName)
    {
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

            if (results.HasValue && results.Value == 1)
                return true;
            else
                return false;
        }
    }
}
