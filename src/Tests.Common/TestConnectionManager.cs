using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.Tests.Common;

public class TestConnectionManager : DatabaseConnectionManager
{
    public TestConnectionManager(IConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    public TestConnectionManager(IDbConnection connection)
        : base(_ => connection)
    {
    }

    public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
    {
        return new[] {scriptContents};
    }
}
