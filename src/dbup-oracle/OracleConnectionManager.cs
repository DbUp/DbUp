using System.Collections.Generic;
using System.Linq;
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
            var commands = new List<string>();
            foreach (var statement in scriptStatements)
            {
                var lowerStatement = statement.ToLower().Trim();
                if (lowerStatement.Contains("begin") || !lowerStatement.Contains(";"))
                {
                    commands.Add(statement);
                } 
                else
                {
                    var subStatements = statement.Split(';').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x));
                    commands.AddRange(subStatements);
                }
            }
            return commands;
        }
    }
}
