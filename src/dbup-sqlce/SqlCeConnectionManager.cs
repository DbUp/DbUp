using System.Collections.Generic;
using System.Data.SqlServerCe;
using DbUp.Engine.Transactions;
using DbUp.Support;

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
        public SqlCeConnectionManager(string connectionString) : base(l => new SqlCeConnection(connectionString))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
