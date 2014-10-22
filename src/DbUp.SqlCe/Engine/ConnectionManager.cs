using System;
using System.Data;
using System.Data.SqlServerCe;
using DbUp.Engine.Output;
using DbUp.Support.SqlServer;

namespace DbUp.SqlCe {
    /// <summary>
    /// Manages SqlCe Database Connections
    /// </summary>
    public class ConnectionManager : SqlConnectionManager {
        private readonly string connectionString;

        /// <summary>
        /// Manages SqlCe Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public ConnectionManager(string connectionString)
            : base(connectionString) {
            this.connectionString = connectionString;
        }

        protected override IDbConnection CreateConnection(IUpgradeLog log) {
            return new SqlCeConnection(connectionString);
        }
    }
}