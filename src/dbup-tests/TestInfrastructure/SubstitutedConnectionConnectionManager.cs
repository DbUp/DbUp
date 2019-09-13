using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Transactions;

namespace DbUp.Tests
{
    class SubstitutedConnectionConnectionManager : DatabaseConnectionManager
    {
        public SubstitutedConnectionConnectionManager(IDbConnection conn) : base(l => conn)
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            yield return scriptContents;
        }
    }
}