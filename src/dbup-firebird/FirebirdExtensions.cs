using DbUp.Builder;
using DbUp.Firebird;
using DbUp.Engine.Transactions;
using DbUp;
using DbUp.Engine.Output;
using FirebirdSql.Data.FirebirdClient;
using System.IO;

// ReSharper disable once CheckNamespace

/// <summary>
/// Configuration extension methods for Firebird.
/// </summary>
public static class FirebirdExtensions
{
    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Firebird database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(this SupportedDatabases supported, string connectionString)
    {
        return FirebirdDatabase(new FirebirdConnectionManager(connectionString));
    }
    
    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="FirebirdConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
        => FirebirdDatabase(connectionManager);

    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="FirebirdConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new FirebirdScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new FirebirdTableJournal(() => c.ConnectionManager, () => c.Log, "SchemaVersions"));
        builder.WithPreprocessor(new FirebirdPreprocessor());
        return builder;
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString)
    {
        FirebirdDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout)
    {
        FirebirdDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, IUpgradeLog logger, int timeout = -1)
    {
        var builder = new FbConnectionStringBuilder(connectionString);

        if (builder.ServerType == FbServerType.Embedded)
        {
            if (!File.Exists(builder.Database))
            {
                FbConnection.CreateDatabase(builder.ToString(), false);
                logger.WriteInformation("Created database {0}", builder.Database);
            } else
                logger.WriteInformation("Database {0} already exists", builder.Database);

        }
        else
        {
            using (var conn = new FbConnection(builder.ToString()))
            {
                try
                {
                    //No way to check if the database exists on the server...
                    conn.Open(); 
                    conn.Close();

                    logger.WriteInformation("Database {0} already exists", builder.Database);
                }
                catch
                {
                    // ... assume the connect failed because the database doesn't exist yet
                    FbConnection.CreateDatabase(builder.ToString(), false);
                    logger.WriteInformation("Created database {0}", builder.Database);
                }
            }
        }
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString)
    {
        FirebirdDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, int commandTimeout)
    {
        FirebirdDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, IUpgradeLog logger, int timeout = -1)
    {
        var builder = new FbConnectionStringBuilder(connectionString);

        if (builder.ServerType == FbServerType.Embedded)
        {
            if (File.Exists(builder.Database))
            {
                FbConnection.DropDatabase(builder.ToString());
                logger.WriteInformation("Dropped database {0}", builder.Database);
            }    
        }
        else
        {
            try
            {
                FbConnection.DropDatabase(builder.ToString());
                logger.WriteInformation("Dropped database {0}", builder.Database);
            }
            catch (FbException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 335544344:
                        logger.WriteInformation("Can't drop database - no database found.");
                        break;
                    case 335544510:
                        logger.WriteError("Can't drop database. Is there still an active connection?");
                        break;
                    default:
                        break;
                }
                // ... assume the connect failed because the database doesn't exist yet, no action
            }
        }
    }
}