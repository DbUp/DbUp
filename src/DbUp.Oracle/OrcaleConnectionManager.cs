using System.Collections.Generic;
using DbUp.Engine.Transactions;
using Oracle.ManagedDataAccess.Client;

namespace DbUp.Oracle
{
    public class OracleConnectionManager : DatabaseConnectionManager
    {
        public OracleConnectionManager(string connectionString) 
            : base(new DelegateConnectionFactory(l => new OracleConnection(connectionString)))
        {
        }
        

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            using (var reader = new OracleCommandReader(scriptContents))
            {
                var commands = new List<string>();
                reader.ReadAllCommands(c => commands.Add(c));
                return commands;
            }
        }
    }
}
