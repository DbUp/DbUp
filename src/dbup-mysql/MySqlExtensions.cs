using System;
using System.Data;
using System.Linq;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.MySql;
using MySql.Data.MySqlClient;

/// <summary>
/// Configuration extension methods for MySql.
/// </summary>
// ReSharper disable once CheckNamespace
public static class MySqlExtensions
{
    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">MySql database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(this SupportedDatabases supported, string connectionString)
    {
        foreach (var pair in connectionString.Split(';').Select(s => s.Split('=')).Where(pair => pair.Length == 2).Where(pair => pair[0].ToLower() == "database"))
        {
            return MySqlDatabase(new MySqlConnectionManager(connectionString), pair[1]);
        }

        return MySqlDatabase(new MySqlConnectionManager(connectionString));
    }

    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">MySql database connection string.</param>
    /// <param name="schema">Which MySql schema to check for changes</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(this SupportedDatabases supported, string connectionString, string schema)
    {
        return MySqlDatabase(new MySqlConnectionManager(connectionString), schema);
    }

    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="MySqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
        => MySqlDatabase(connectionManager);

    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="MySqlConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(IConnectionManager connectionManager)
    {
        return MySqlDatabase(connectionManager, null);
    }

    /// <summary>
    /// Creates an upgrader for MySql databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="MySqlConnectionManager"/> to be used during a database upgrade.</param>
    /// /// <param name="schema">Which MySQL schema to check for changes</param>
    /// <returns>
    /// A builder for a database upgrader designed for MySql databases.
    /// </returns>
    public static UpgradeEngineBuilder MySqlDatabase(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new MySqlScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new MySqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, "schemaversions"));
        builder.WithPreprocessor(new MySqlPreprocessor());
        return builder;
    }


    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void MySqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString)
    {
        MySqlDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void MySqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout)
    {
        MySqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void MySqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, string collation)
    {
        MySqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), collation: collation);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void MySqlDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, string collation)
    {
        MySqlDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout, collation: collation);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void MySqlDatabase(
        this SupportedDatabasesForEnsureDatabase supported,
        string connectionString,
        IUpgradeLog logger,
        int timeout = -1,
        string collation = null)
    {
        GetMysqlConnectionStringBuilder(connectionString, logger, out var masterConnectionString, out var databaseName);

        try
        {
            using (var connection = new MySqlConnection(masterConnectionString))
            {
                connection.Open();
                if (DatabaseExists(connection, databaseName))
                    return;
            }
        }
        catch (Exception e)
        {
            logger.WriteInformation(@"Database not found on server with connection string in settings: {0}", e.Message);
        }

        using (var connection = new MySqlConnection(masterConnectionString))
        {
            connection.Open();
            if (DatabaseExists(connection, databaseName))
                return;

            var collationString = string.IsNullOrEmpty(collation) ? "" : string.Format(@" COLLATE {0}", collation);
            var sqlCommandText = string.Format
                    (
                        @"create database {0}{1};",
                        databaseName,
                        collationString
                    );


            // Create the database...
            using (var command = new MySqlCommand(sqlCommandText, connection)
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

    static bool DatabaseExists(MySqlConnection connection, string databaseName)
    {
        var sqlCommandText = string.Format
        (
            $"SELECT SCHEMA_NAME FROM information_schema.schemata WHERE SCHEMA_NAME = '{databaseName}';"
        );

        // check to see if the database already exists..
        using (var command = new MySqlCommand(sqlCommandText, connection)
        {
            CommandType = CommandType.Text
        })
        {
            var result = command.ExecuteScalar();
            return result != null;
        }
    }

    static void GetMysqlConnectionStringBuilder(string connectionString, IUpgradeLog logger, out string masterConnectionString, out string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim() == string.Empty)
            throw new ArgumentNullException("connectionString");

        if (logger == null)
            throw new ArgumentNullException("logger");

        var masterConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
        databaseName = masterConnectionStringBuilder.Database;

        if (string.IsNullOrEmpty(databaseName) || databaseName.Trim() == string.Empty)
            throw new InvalidOperationException("The connection string does not specify a database name.");

        masterConnectionStringBuilder.Database = "mysql";
        masterConnectionString = masterConnectionStringBuilder.ConnectionString;
    }
}
