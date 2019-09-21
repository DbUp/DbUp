using System.Collections.Generic;
using DbUp.Engine.Transactions;
using Oracle.ManagedDataAccess.Client;

namespace DbUp.Oracle
{
    public class OracleConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Creates a new Oracle database connection.
        /// </summary>
        /// <param name="connectionString">The Oracle connection string.</param>
        public OracleConnectionManager(string connectionString) : base(new DelegateConnectionFactory(l => new OracleConnection(connectionString)))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new OracleCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
