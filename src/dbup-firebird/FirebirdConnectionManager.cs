using DbUp.Engine.Transactions;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using System.Collections.Generic;

namespace DbUp.Firebird
{
    /// <summary>
    /// Manages Firebird database connections.
    /// </summary>
    public class FirebirdConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Creates a new Firebird database connection.
        /// </summary>
        /// <param name="connectionString">The Firebird connection string.</param>
        public FirebirdConnectionManager(string connectionString) : base(new DelegateConnectionFactory(l => new FbConnection(connectionString)))
        {
        }

        /// <summary>
        /// Splits the statements in the script using the ";" character.
        /// </summary>
        /// <param name="scriptContents">The contents of the script to split.</param>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var statements = new List<string>();

            var script = new FbScript(scriptContents);
            script.Parse();
            
            foreach (FbStatement stmt in script.Results)
                statements.Add(stmt.Text);

            return statements.ToArray();
        }
    }
}
