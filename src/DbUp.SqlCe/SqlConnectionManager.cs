using System;
using System.Data;
using System.Data.SqlServerCe;
using DbUp.Support.SqlServer;

namespace DbUp.SqlCe
{
    /// <summary>
    /// Manages SqlCe Database Connections
    /// </summary>
    public class SqlCeConnectionManager : SqlConnectionManager
    {
        private readonly string connectionString;

        /// <summary>
        /// Manages SqlCe Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlCeConnectionManager(string connectionString) : base(connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override IDbConnection CreateConnection()
        {
            return new SqlCeConnection(connectionString);
        }
    }
}