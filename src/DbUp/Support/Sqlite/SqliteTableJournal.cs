using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;

namespace DbUp.Support.Sqlite
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// SQLite database using a table called SchemaVersions.
    /// </summary>
    public class SQLiteTableJournal : IJournal
    {
        private readonly Func<IDbConnection> connectionFactory;
        private readonly string tableName;
        private readonly IUpgradeLog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableJournal"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="table">The table name.</param>
        /// <param name="logger">The log.</param>
        public SQLiteTableJournal(Func<IDbConnection> connectionFactory, string table, IUpgradeLog logger)
        {
            this.connectionFactory = connectionFactory;
            tableName = table;
            log = logger;
        }

        /// <summary>
        /// Recalls the version number of the database.
        /// </summary>
        /// <returns>All executed scripts.</returns>
        public string[] GetExecutedScripts()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Records a database upgrade for a database specified in a given connection string.
        /// </summary>
        /// <param name="script">The script.</param>
        public void StoreExecutedScript(SqlScript script)
        {
            throw new System.NotImplementedException();
        }
    }
}
