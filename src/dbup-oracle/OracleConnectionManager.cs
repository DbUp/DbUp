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
        /// Creates a new Oracle database connection manager.
        /// </summary>
        /// <param name="connectionString">The Oracle connection string.</param>
        [Obsolete("Use OracleConnectionManager(string, OracleCommandSplitter) and supply an appropriate command splitter instance.")]
        public OracleConnectionManager(string connectionString)
            : this(connectionString, new OracleCommandSplitter())
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Creates a new Oracle database connection manager.
        /// </summary>
        /// <param name="connectionString">The Oracle connection string.</param>
        /// <param name="commandSplitter">A class that splits a string into individual Oracle SQL statements.</param>
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
