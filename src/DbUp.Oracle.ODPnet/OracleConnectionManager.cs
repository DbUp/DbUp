using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace DbUp.Oracle.ODPnet
{
        public class OracleConnectionManager : DatabaseConnectionManager
       {
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            // if we have a shared connection, return it, otherwise create a connection
            return new OracleConnection(ConnectionString);
        }

        /// <summary>
        /// Oracle statements seprator is ???
        /// </summary>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            return new List<string>() { scriptContents };
        }
    }
}
