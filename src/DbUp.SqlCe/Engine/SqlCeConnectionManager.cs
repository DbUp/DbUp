using System;
using System.Data;
using System.Data.SqlServerCe;
using DbUp.Engine.Output;
using DbUp.Support.SqlServer;
using DbUp.SqlCe.Engine;

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
            this.SqlContainer = new SqlCeStatements();
        }

        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new SqlCeConnection(connectionString);
        }
    }
}
