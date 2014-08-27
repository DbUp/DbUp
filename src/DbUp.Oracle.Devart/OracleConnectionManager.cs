using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using Devart.Data.Oracle;

namespace DbUp.Oracle.Devart
{
    public class DevartOracleConnectionManager : DatabaseConnectionManager
       {
        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return new OracleConnection(ConnectionString);
        }

        /// <summary>
        /// Split script using OracleScript class from Devart.
        /// </summary>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            OracleScript script = new OracleScript(scriptContents);
            IEnumerable<string> scriptStatements = from OracleSqlStatement statement in script.Statements 
                                                   select statement.Text;
            return scriptStatements;
        }
    }
}
