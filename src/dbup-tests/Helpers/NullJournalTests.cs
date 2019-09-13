using System.Data;
using DbUp.Helpers;
using NSubstitute;
using Shouldly;
using Xunit;
#pragma warning disable 618

namespace DbUp.Tests.Helpers
{
    public class NullJournalTests
    {
        [Fact]
        public void shouldnt_journal_anything_executed()
        {
            var journal = new NullJournal();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "SELECT * FROM BLAH")
                .JournalTo(journal)
                .Build();

            upgradeEngine.PerformUpgrade();

            journal.GetExecutedScripts().ShouldBeEmpty();
        }
    }
}