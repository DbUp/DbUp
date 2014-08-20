using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.QueryProviders;
using DbUp.Support.SqlServer;

namespace DbUp.SqlCe
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a SQLite database
    /// </summary>
    public sealed class SqlCeTableJournal: SqlTableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeTableJournal"/> class.
        /// </summary>
        public SqlCeTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger) :
            base(connectionManager, logger)
        {
            QueryProvider = new SqlCeQueryProvider();
        }
    }
}

