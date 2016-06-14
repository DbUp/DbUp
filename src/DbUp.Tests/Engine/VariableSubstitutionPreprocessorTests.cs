using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbUp.Engine;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Engine
{
    [TestFixture]
    public class VariableSubstitutionPreprocessorTests
    {
        [Test]
        public void substitutes_variables_in_body()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "something $somevar$ something")
                .JournalTo(journal)
                .WithVariable("somevar", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual("something coriander something", command.CommandText);
        }

        [Test]
        public void ignores_variables_in_quoted_text()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(() => connection, "Db")
                .WithScript("testscript", "'$somevar$'")
                .JournalTo(journal)
                .WithVariable("somevar", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            Assert.AreEqual("'$somevar$'", command.CommandText);
        }

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
                .WithVariable("beansprouts", "coriander")
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
                .WithVariable("beansprouts", "coriander")
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
                .WithVariable("beansprouts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            result.Successful.ShouldBeTrue();
            command.CommandText.ShouldBe("--$somevar$");
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
                .WithVariable("beansprouts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            Assert.IsFalse(result.Successful);
            Assert.IsInstanceOf<InvalidOperationException>(result.Error);
        }
    }
}
