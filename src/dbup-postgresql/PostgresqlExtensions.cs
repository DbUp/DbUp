using System;
using System.Data;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Postgresql;
using DbUp.Engine.Transactions;
using Npgsql;
using System.Security.Cryptography.X509Certificates;

// ReSharper disable once CheckNamespace

/// <summary>
/// Configuration extension methods for PostgreSQL.
/// </summary>
public static class PostgresqlExtensions
{
    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">PostgreSQL database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(this SupportedDatabases supported, string connectionString) 
        => PostgresqlDatabase(supported, connectionString, null);

    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">PostgreSQL database connection string.</param>
    /// <param name="schema">The schema in which to check for changes</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(this SupportedDatabases supported, string connectionString, string schema) 
        => PostgresqlDatabase(new PostgresqlConnectionManager(connectionString), schema);

    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="PostgresqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
        => PostgresqlDatabase(connectionManager);
    
    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="PostgresqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(IConnectionManager connectionManager)
        => PostgresqlDatabase(connectionManager, null);
    
    /// <summary>
    /// Creates an upgrader for PostgreSQL databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="PostgresqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <param name="schema">The schema in which to check for changes</param>
    /// <returns>
    /// A builder for a database upgrader designed for PostgreSQL databases.
    /// </returns>
    public static UpgradeEngineBuilder PostgresqlDatabase(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new PostgresqlScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new PostgresqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, "schemaversions"));
        builder.WithPreprocessor(new PostgresqlPreprocessor());
        return builder;
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="certificateFile">Optional SSL client pfx certificate for db.</param>
    /// <returns></returns>
    public static void PostgresqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, string certificateFile = null)
    {
        PostgresqlDatabase(supported, connectionString, certificateFile, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="certificateFile">Optional SSL client pfx certificate for db.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <returns></returns>
    public static void PostgresqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, string certificateFile, IUpgradeLog logger)
    {
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim() == string.Empty)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        if (logger == null) throw new ArgumentNullException(nameof(logger));

        GetMasterConnectionStringBuilder(connectionString, logger, out var masterConnectionString, out var databaseName);

        using (var connection = new NpgsqlConnection(masterConnectionString))
        {
            if (!string.IsNullOrEmpty(certificateFile) && certificateFile.Trim() != string.Empty)
                connection.ProvideClientCertificatesCallback += certs => certs.Add(new X509Certificate2(certificateFile));

            connection.Open();

            var sqlCommandText = string.Format
            (
                @"SELECT case WHEN oid IS NOT NULL THEN 1 ELSE 0 end FROM pg_database WHERE datname = '{0}' limit 1;",
                databaseName
            );


            // check to see if the database already exists..
            using (var command = new NpgsqlCommand(sqlCommandText, connection)
            {
                CommandType = CommandType.Text
            })
            {
                var results = (int?) command.ExecuteScalar();

                // if the database exists, we're done here...
                if (results.HasValue && results.Value == 1)
                {
                    return;
                }
            }

            sqlCommandText = string.Format
            (
                "create database \"{0}\";",
                databaseName
            );

            // Create the database...
            using (var command = new NpgsqlCommand(sqlCommandText, connection)
            {
                CommandType = CommandType.Text
            })
            {
                command.ExecuteNonQuery();

            }

            logger.WriteInformation(@"Created database {0}", databaseName);
        }
    }
    
    /// <summary>
    /// Tracks the list of executed scripts in a SQL Server table.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder JournalToPostgresqlTable(this UpgradeEngineBuilder builder, string schema, string table)
    {
        builder.Configure(c => c.Journal = new PostgresqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, table));
        return builder;
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="certificateFile">Optional SSL client pfx certificate for db.</param>
    /// <returns></returns>
    public static void PostgresqlDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, string certificateFile = null)
    {
        PostgresqlDatabase(supported, connectionString, certificateFile, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="certificateFile">Client certificate for db.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void PostgresqlDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, string certificateFile, IUpgradeLog logger, int timeout = -1)
    {
        GetMasterConnectionStringBuilder(connectionString, logger, out var masterConnectionString, out var databaseName);

        using (var connection = new NpgsqlConnection(masterConnectionString))
        {
            if (!string.IsNullOrEmpty(certificateFile) && certificateFile.Trim() != string.Empty)
                connection.ProvideClientCertificatesCallback += certs => certs.Add(new X509Certificate2(certificateFile));

            connection.Open();
            using (var command = new NpgsqlCommand($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = \'{databaseName}\'; DROP DATABASE IF EXISTS \"{databaseName}\";", connection))
            {
                command.ExecuteNonQuery();
            }

            logger.WriteInformation("Dropped database {0}", databaseName);
        }
    }

    private static void GetMasterConnectionStringBuilder(string connectionString, IUpgradeLog logger, out string masterConnectionString, out string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim() == string.Empty)
            throw new ArgumentNullException(nameof(connectionString));

        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        var masterConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        databaseName = masterConnectionStringBuilder.Database;

        if (string.IsNullOrEmpty(databaseName) || databaseName.Trim() == string.Empty)
        {
            throw new InvalidOperationException("The connection string does not specify a database name.");
        }

        masterConnectionStringBuilder.Database = "postgres";

        var logMasterConnectionStringBuilder = new NpgsqlConnectionStringBuilder(masterConnectionStringBuilder.ConnectionString);
        if (!string.IsNullOrEmpty(masterConnectionStringBuilder.Password))
        {
            logMasterConnectionStringBuilder.Password = string.Empty.PadRight(masterConnectionStringBuilder.Password.Length, '*');
        }

        logger.WriteInformation("Master ConnectionString => {0}", logMasterConnectionStringBuilder.ConnectionString);

        masterConnectionString = masterConnectionStringBuilder.ConnectionString;
    }
}