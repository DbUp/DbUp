using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using NSubstitute;
using DbUp.Support.SqlServer;

namespace DbUp.Tests
{
    public class TestConnectionManager : DatabaseConnectionManager
    {
        private readonly IDbConnection connection;

        public TestConnectionManager(IDbConnection connection = null, bool startUpgrade = false)
        {
            this.connection = connection ?? Substitute.For<IDbConnection>();
            if (startUpgrade)
                OperationStarting(new ConsoleUpgradeLog(), new List<SqlScript>());
            this._sqlContainer = new SqlServerStatementsContainer();
        }

        protected override IDbConnection CreateConnection(IUpgradeLog log)
        {
            return connection;
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            return new[] {scriptContents};
        }
    }
}