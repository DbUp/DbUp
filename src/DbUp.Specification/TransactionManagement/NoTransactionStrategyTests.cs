using System;
using System.Data;
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
            strategy = new NoTransactionStrategy(() => connection);
        }

        [Test]
        public void should_open_a_connection_per_request()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(2).Open();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(4).Open();
        }

        [Test]
        public void should_dispose_a_connection_per_request()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(2).Dispose();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(4).Dispose();
        }
    }
}