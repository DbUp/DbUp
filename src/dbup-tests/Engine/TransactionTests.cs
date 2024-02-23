using System;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Tests.Common;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

#pragma warning disable 618

namespace DbUp.Tests.Engine;

public class TransactionTests
{
    [Fact]
    public void OnlyOneTransactionIsOpenedAtATimeWithTransactionPerScriptStrategy()
    {
        var connectionFactory = new TransactionCountingConnectionFactory();

        var testProvider = new TestProvider();
        testProvider.Builder.Configure(c => c.ConnectionManager = new TestConnectionManager(connectionFactory));

        var upgradeEngine = testProvider.Builder
            .WithScript("testscript1", "SELECT 1")
            .WithScript("testscript2", "SELECT 1")
            .WithTransactionPerScript()
            .Build();

        var result = upgradeEngine.PerformUpgrade();
        result.Error.ShouldBeNull();
        result.Successful.ShouldBeTrue();

        connectionFactory.TransactionWasOpened.ShouldBeTrue("BeginTransaction was never called");
    }

    class TransactionCountingConnectionFactory : IConnectionFactory
    {
        int transactionCount;

        public bool TransactionWasOpened { get; private set; }

        public IDbConnection CreateConnection(IUpgradeLog upgradeLog, DatabaseConnectionManager databaseConnectionManager)
        {
            var conn = Substitute.For<IDbConnection>();
            conn.BeginTransaction().ReturnsForAnyArgs(c =>
            {
                TransactionWasOpened = true;

                if (transactionCount > 0)
                    throw new Exception("Test failed as multiple transaction were opened");

                transactionCount++;
                var trn = Substitute.For<IDbTransaction>();
                trn.When(_ => _.Dispose()).Do(_ => transactionCount--);
                return trn;
            });
            return conn;
        }
    }
}
