using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Engine.Transactions;
using Google.Cloud.Spanner.Data;

namespace DbUp.Spanner
{
    public class SpannerConnectionManager : DatabaseConnectionManager
    {
        public SpannerConnectionManager(string connectionString) : base(new DelegateConnectionFactory(p => new SpannerConnection(connectionString)))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SpannerCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
