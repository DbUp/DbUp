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

        public TestConnectionManager(IDbConnection connection = null, bool startUpgrade = false) : base((string)null)
        {
            this.connection = connection ?? Substitute.For<IDbConnection>();
            if (startUpgrade)
                UpgradeStarting(new ConsoleUpgradeLog());
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return connection;
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            return new[] {scriptContents};
        }
    }
}