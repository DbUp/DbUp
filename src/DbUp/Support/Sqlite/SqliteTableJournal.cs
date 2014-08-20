﻿using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.QueryProviders;
using DbUp.Support.SqlServer;

namespace DbUp.Support.SQLite
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a SQLite database
    /// </summary>
    public sealed class SQLiteTableJournal : SqlTableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTableJournal"/> class.
        /// </summary>
        public SQLiteTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger) :
            base(connectionManager, logger)
        {
            QueryProvider = new SqliteQueryProvider();
        }
    }
}
