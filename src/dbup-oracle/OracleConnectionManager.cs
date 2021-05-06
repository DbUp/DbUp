using System;
using System.Collections.Generic;
using DbUp.Engine.Transactions;
using Oracle.ManagedDataAccess.Client;

namespace DbUp.Oracle
{
    public class OracleConnectionManager : DatabaseConnectionManager
    {
        private readonly OracleCommandSplitter commandSplitter;

        /// <summary>
        /// Creates a new Oracle database connection.
        /// </summary>
        /// <param name="connectionString">The Oracle connection string.</param>
        [Obsolete]
        public OracleConnectionManager(string connectionString)
            : this(connectionString, new OracleCommandSplitter())
        {
            Console.WriteLine();
        }
        
        public OracleConnectionManager(string connectionString, OracleCommandSplitter commandSplitter)
            : base(new DelegateConnectionFactory(l => new OracleConnection(connectionString)))
        {
            this.commandSplitter = commandSplitter;
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
