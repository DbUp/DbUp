using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbUp.Engine;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Engine
{
    [TestFixture]
    public class VariableSubstitutionPreprocessorTests
    {
        [Test]
        public void ignores_undefined_variables_in_comments()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "/*$somevar$*/")
                .JournalTo(journal)
                .WithVariable("beansprounts", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual("/*$somevar$*/", command.CommandText);
        }
        [Test]
        public void ignores_undefined_variables_in_complex_comments()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "/*/**/$somevar$*/")
                .JournalTo(journal)
                .WithVariable("beansprounts", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual("/*/**/$somevar$*/", command.CommandText);
        }

        [Test]
        public void ignores_undefined_variable_in_line_comment()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "--$somevar$")
                .JournalTo(journal)
                .WithVariable("beansprounts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            Assert.IsFalse(result.Successful);
            Assert.IsInstanceOf<InvalidOperationException>(result.Error);
        }

        [Test]
        public void throws_for_undefined_variable()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "$somevar$")
                .JournalTo(journal)
                .WithVariable("beansprounts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            Assert.IsFalse(result.Successful);
            Assert.IsInstanceOf<InvalidOperationException>(result.Error);
        }
    }
}
