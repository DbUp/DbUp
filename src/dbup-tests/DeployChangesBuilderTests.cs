using System;
using System.Data;
using DbUp.Engine;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class DeployChangesBuilderTests
    {
        [Fact]
        public void can_use_variables_with_builder()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "$schema$Up $somevar$")
                .JournalTo(journal)
                .WithVariable("somevar", "is awesome")
                .Build();

            upgradeEngine.PerformUpgrade();

            command.CommandText.ShouldBe("[Db]Up is awesome");
        }

        [Fact]
        public void WithExecutionTimeout_Should_Set_CommandTimeout_Property_To_Given_Value()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection))
                .WithScript("testscript", "test")
                .JournalTo(journal)
                .WithExecutionTimeout(TimeSpan.FromSeconds(45))
                .Build();

            upgradeEngine.PerformUpgrade();

            command.CommandTimeout.ShouldBe(45);
        }

        [Fact]
        public void WithExecutionTimeout_Should_Not_Set_CommandTimeout_Property_For_Null()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection))
                .WithScript("testscript", "test")
                .JournalTo(journal)
                .WithExecutionTimeout(null)
                .Build();

            upgradeEngine.PerformUpgrade();

            command.CommandTimeout.ShouldBe(0);
        }

        [Fact]
        public void WithExecutionTimeout_Should_Not_Allow_Negative_Timeout_Values()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var upgradeEngine = DeployChanges.To
                    .SqlDatabase(new SubstitutedConnectionConnectionManager(connection))
                    .WithScript("testscript", "test")
                    .JournalTo(journal)
                    .WithExecutionTimeout(TimeSpan.FromSeconds(-5))
                    .Build();
            });
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
