using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.Tests.TestInfrastructure
{
    public class TestConnectionManager : DatabaseConnectionManager
    {
        public TestConnectionManager(IDbConnection connection, bool startUpgrade = false) : base(() => connection)
        {
            if (startUpgrade)
                OperationStarting(new List<SqlScript>());
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            return new[] {scriptContents};
        }
    }
}