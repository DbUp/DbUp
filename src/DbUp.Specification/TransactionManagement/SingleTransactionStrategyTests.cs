using System;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.TransactionManagement
{
    public class SingleTransactionStrategyTests
    {
        private IDbConnection connection;
        private SingleTrasactionStrategy strategy;
        private IDbTransaction transaction;

        [SetUp]
        public void Setup()
        {
            connection = Substitute.For<IDbConnection>();
            transaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(transaction);
            strategy = new SingleTrasactionStrategy(() => connection);
            strategy.Initialise(new ConsoleUpgradeLog());
        }

        [Test]
        public void should_start_a_transaction_once_initialised()
        {
            connection.Received(1).Open();
            connection.Received(1).BeginTransaction();
        }

        [Test]
        public void should_not_open_connection_on_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.Received(1).Open();
        }

        [Test]
        public void should_not_open_dispose_on_execute()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            connection.DidNotReceive().Dispose();
        }

        [Test]
        public void cannot_execute_once_error_occurs()
        {
            Assert.Throws<ArgumentException>(() => strategy.Execute(c => { throw new ArgumentException(); }));

            Assert.Throws<InvalidOperationException>(() => strategy.Execute(c => { }));
        }

        [Test]
        public void cannot_execute_once_error_occurs_with_return_value()
        {
            Assert.Throws<ArgumentException>(() => strategy.Execute<bool>(c => { throw new ArgumentException(); }));

            Assert.Throws<InvalidOperationException>(() => strategy.Execute(c => true));
        }

        [Test]
        public void commits_transaction_when_no_errors_occured()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            strategy.Dispose();

            transaction.Received().Commit();
        }

        [Test]
        public void does_not_commit_transaction_when_error_occured()
        {
            Assert.Throws<ArgumentException>(() => strategy.Execute(c => { throw new ArgumentException(); }));
            strategy.Dispose();

            transaction.DidNotReceive().Commit();
        }

        [Test]
        public void disposes_transaction_and_connection_on_dispose()
        {
            strategy.Execute(c => { });
            strategy.Execute(c => true);
            strategy.Dispose();

            transaction.Received().Dispose();
            connection.Received().Dispose();
        }
    }
}