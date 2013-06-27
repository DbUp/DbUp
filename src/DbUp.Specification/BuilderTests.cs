using System;
using System.Data;
using DbUp.Engine;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests
{
    [TestFixture]
    public class BuilderTests
    {
        [Test]
        public void can_use_variables_with_builder()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "$schema$Up $somevar$")
                .JournalTo(journal)
                .WithVariable("somevar", "is awesome")
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual("[Db]Up is awesome", command.CommandText);
        }

        [Test]
        public void WithExecutionTimeout_Should_Set_CommandTimeout_Property_To_Given_Value()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection)
                .WithScript("testscript", "test")
                .JournalTo(journal)
                .WithExecutionTimeout(TimeSpan.FromSeconds(45))
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual(45, command.CommandTimeout);
        }

        [Test]
        public void WithExecutionTimeout_Should_Not_Set_CommandTimeout_Property_For_Null()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection)
                .WithScript("testscript", "test")
                .JournalTo(journal)
                .WithExecutionTimeout(null)
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual(0, command.CommandTimeout);
        }


        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WithExecutionTimeout_Should_Not_Allow_Negative_Timeout_Values()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection)
                .WithScript("testscript", "test")
                .JournalTo(journal)
                .WithExecutionTimeout(TimeSpan.FromSeconds(-5))
                .Build();
        }
    }
}
