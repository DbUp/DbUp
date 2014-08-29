using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using Devart.Data.Oracle;

namespace OracleSampleApplication
{
    public class DevartOracleConnectionManager : DatabaseConnectionManager
       {

        private static readonly DevartOracleConnectionManager instance = new DevartOracleConnectionManager();

           /// <summary>
           /// Returns the databases supported by DbUp.
           /// </summary>
        public static DevartOracleConnectionManager Instance
           {
               get { return instance; }
           }

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
