using System;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.TransactionManagement
{
    public class NoTransactionStrategyTests
    {
        private IDbConnection connection;
        private NoTransactionStrategy strategy;

        [SetUp]
        public void Setup()
        {
            connection = Substitute.For<IDbConnection>();
            strategy = new NoTransactionStrategy();
            strategy.Initialise(connection, new ConsoleUpgradeLog());
        }

        [Test]
        public void should_not_open_a_connection_per_request()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.DidNotReceive().Open();
        }

        [Test]
        public void should_dispose_connection()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.DidNotReceive().Dispose();
        }
    }
}