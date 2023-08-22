using System;
using System.Linq;
using DbUp.Builder;
using DbUp.Engine.Transactions;

namespace DbUp.Oracle
{
#pragma warning disable IDE0060 // Remove unused parameter - The "SupportedDatabases" parameter is never used.
    public static class OracleExtensions
    {
        /// <summary>
        /// Create an upgrader for Oracle databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        /// <remarks>
        /// This method is obsolete. One of these should be used instead:
        /// <list type="bullet">
        /// <item><see cref="OracleDatabaseWithDefaultDelimiter(SupportedDatabases, string)"/></item>
        /// <item><see cref="OracleDatabaseWithSemicolonDelimiter(SupportedDatabases, string)"/></item>
        /// <item><see cref="OracleDatabase(SupportedDatabases, string, char)"/></item>
        /// </list>
        /// </remarks>
        [Obsolete("Use OracleDatabaseWithDefaultDelimiter, OracleDatabaseWithSemicolonDelimiter or the OracleDatabase with the delimiter parameter instead, see https://github.com/DbUp/DbUp/pull/335")]
        public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString)
        {
            foreach (var pair in connectionString.Split(';').Select(s => s.Split('=')).Where(pair => pair.Length == 2).Where(pair => pair[0].ToLower() == "database"))
            {
                return OracleDatabase(new OracleConnectionManager(connectionString), pair[1]);
            }

            return OracleDatabase(new OracleConnectionManager(connectionString));
        }

        /// <summary>
        /// Create an upgrader for Oracle databases that uses the <c>/</c> character as the delimiter between statements.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabaseWithDefaultDelimiter(this SupportedDatabases supported, string connectionString)
            => OracleDatabase(supported, connectionString, '/');

        /// <summary>
        /// Create an upgrader for Oracle databases that uses the <c>;</c> character as the delimiter between statements.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabaseWithSemicolonDelimiter(this SupportedDatabases supported, string connectionString)
            => OracleDatabase(supported, connectionString, ';');

        /// <summary>
        /// Create an upgrader for Oracle databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="delimiter">Character to use as the delimiter between statements.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString, char delimiter)
        {
            foreach (var pair in connectionString.Split(';').Select(s => s.Split('=')).Where(pair => pair.Length == 2).Where(pair => pair[0].ToLower() == "database"))
            {
                return OracleDatabase(new OracleConnectionManager(connectionString, new OracleCommandSplitter(delimiter)), pair[1]);
            }

            return OracleDatabase(new OracleConnectionManager(connectionString, new OracleCommandSplitter(delimiter)));
        }

        /// <summary>
        /// Create an upgrader for Oracle databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="delimiter">Character to use as the delimiter between statements.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString, string schema, char delimiter)
        {
            return OracleDatabase(new OracleConnectionManager(connectionString, new OracleCommandSplitter(delimiter)), schema);
        }

        /// <summary>
        /// Creates an upgrader for Oracle databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">Oracle database connection string.</param>
        /// <param name="schema">Which Oracle schema to check for changes</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        [Obsolete("Use the parameter that takes a delimiter instead, see https://github.com/DbUp/DbUp/pull/335")]
        public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString, string schema)
        {
            return OracleDatabase(new OracleConnectionManager(connectionString), schema);
        }

        /// <summary>
        /// Creates an upgrader for Oracle databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">Oracle database connection string.</param>
        /// <param name="schema">Which Oracle schema to check for changes</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        /// <remarks>
        /// This method is obsolete. One of these should be used instead:
        /// <list type="bullet">
        /// <item><see cref="OracleDatabaseWithDefaultDelimiter(SupportedDatabases, string)"/></item>
        /// <item><see cref="OracleDatabaseWithSemicolonDelimiter(SupportedDatabases, string)"/></item>
        /// <item><see cref="OracleDatabase(SupportedDatabases, string, char)"/></item>
        /// </list>
        /// </remarks>
        [Obsolete("Use OracleDatabaseWithDefaultDelimiter, OracleDatabaseWithSemicolonDelimiter or the OracleDatabase with the delimiter parameter instead, see https://github.com/DbUp/DbUp/pull/335")]
        public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, string connectionString, string schema, string delimiter)
        {
            return OracleDatabase(new OracleConnectionManager(connectionString), schema);
        }

        /// <summary>
        /// Creates an upgrader for Oracle databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionManager">The <see cref="OracleConnectionManager"/> to be used during a database upgrade.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
        {
            return OracleDatabase(connectionManager);
        }

        /// <summary>
        /// Creates an upgrader for Oracle databases.
        /// </summary>
        /// <param name="connectionManager">The <see cref="OracleConnectionManager"/> to be used during a database upgrade.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabase(IConnectionManager connectionManager)
        {
            return OracleDatabase(connectionManager, null);
        }

        /// <summary>
        /// Creates an upgrader for Oracle databases.
        /// </summary>
        /// <param name="connectionManager">The <see cref="OracleConnectionManager"/> to be used during a database upgrade.</param>
        /// /// <param name="schema">Which Oracle schema to check for changes</param>
        /// <returns>
        /// A builder for a database upgrader designed for Oracle databases.
        /// </returns>
        public static UpgradeEngineBuilder OracleDatabase(IConnectionManager connectionManager, string schema)
        {
            var builder = new UpgradeEngineBuilder();
            builder.Configure(c => c.ConnectionManager = connectionManager);
            builder.Configure(c => c.ScriptExecutor = new OracleScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
            builder.Configure(c => c.Journal = new OracleTableJournal(() => c.ConnectionManager, () => c.Log, schema, "schemaversions"));
            builder.WithPreprocessor(new OraclePreprocessor());
            return builder;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
