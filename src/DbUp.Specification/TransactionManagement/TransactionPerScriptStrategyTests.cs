using System;
using System.Data;
using DbUp.Engine.Transactions;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.TransactionManagement
{
    public class TransactionPerScriptStrategyTests
    {
        private IDbConnection connection;
        private TransactionPerScriptStrategy strategy;
        private IDbTransaction transaction;

        [SetUp]
        public void Setup()
        {
            connection = Substitute.For<IDbConnection>();
            transaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(transaction);
            strategy = new TransactionPerScriptStrategy(() => connection);
        }

        [Test]
        public void should_open_connection_on_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(2).Open();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(4).Open();
        }

        [Test]
        public void begins_transaction_on_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(2).BeginTransaction();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(4).BeginTransaction();
        }

        [Test]
        public void commits_transaction_on_successful_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            transaction.Received(2).Commit();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            transaction.Received(4).Commit();
        }

        [Test]
        public void does_not_commit_transaction_on_error()
        {
            Assert.Throws<ArgumentException>(() => strategy.Execute(c => { throw new ArgumentException(); }));
            Assert.Throws<ArgumentException>(() => strategy.Execute<bool>(c => { throw new ArgumentException(); }));

            transaction.DidNotReceive().Commit();
        }

        [Test]
        public void disposes_connection_on_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(2).Dispose();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(4).Dispose();
        }

        [Test]
        public void disposes_transaction_on_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            transaction.Received(2).Dispose();

            strategy.Execute(c => { });
            strategy.Execute(c => true);
            transaction.Received(4).Dispose();
        }
    }
}