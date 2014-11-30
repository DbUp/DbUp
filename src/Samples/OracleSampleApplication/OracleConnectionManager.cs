using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using Devart.Data.Oracle;

namespace OracleSampleApplication
{
    /// <summary>
    /// Using Devart Oracle Provider for overriding basic Oracle provider ODP.NET
    /// </summary>
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
            var scriptStatements =
                System.Text.RegularExpressions.Regex.Split(scriptContents, "/\r*$", System.Text.RegularExpressions.RegexOptions.Multiline)
               .Select(x => x.Trim())
               .Where(x => x.Length > 0)
               .ToArray();

            return scriptStatements;
            //OracleScript script = new OracleScript(scriptContents);
            //IEnumerable<string> scriptStatements = from OracleSqlStatement statement in script.Statements 
            //                                       select statement.Text;
            //return scriptStatements;
        }
    }
}
