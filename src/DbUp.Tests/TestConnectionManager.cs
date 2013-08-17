using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using NSubstitute;

namespace DbUp.Tests
{
    public class TestConnectionManager : DatabaseConnectionManager
    {
        private readonly IDbConnection connection;

        public TestConnectionManager(IDbConnection connection = null, bool startUpgrade = false)
        {
            this.connection = connection ?? Substitute.For<IDbConnection>();
            if (startUpgrade)
                OperationStarting(new ConsoleUpgradeLog());
        }

        protected override IDbConnection CreateConnection()
        {
            return connection;
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            return new[] {scriptContents};
        }
    }
}