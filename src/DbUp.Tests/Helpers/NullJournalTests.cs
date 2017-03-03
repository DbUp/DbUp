using System.Data;
using DbUp.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Helpers
{
    [TestFixture]
    public class NullJournalTests
    {
        [Test]
        public void shouldnt_journal_anything_executed()
        {
            var journal = new NullJournal();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "SELECT * FROM BLAH")
                .JournalTo(journal)
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual(0, journal.GetExecutedScripts().Length);
        }
    }
}
