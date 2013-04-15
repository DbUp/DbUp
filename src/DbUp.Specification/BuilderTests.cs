using System;
using System.Data;
using DbUp.Engine;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification
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
                .SqlDatabase(()=>connection, "Db")
                .WithScript("testscript", "$schema$Up $somevar$")
                .JournalTo(journal)
                .WithVariable("somevar", "is awesome")
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual("[Db]Up is awesome", command.CommandText);
        }
    }
}
