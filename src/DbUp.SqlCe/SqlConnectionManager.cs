using System;
using System.Data;
using System.Data.SqlServerCe;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.SqlCe
{
    /// <summary>
    /// Manages SqlCe Database Connections
    /// </summary>
    public class SqlCeConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Manages SqlCe Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlCeConnectionManager(string connectionString) : base(connectionString)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlCeConnection(connectionString);
        }
    }
}