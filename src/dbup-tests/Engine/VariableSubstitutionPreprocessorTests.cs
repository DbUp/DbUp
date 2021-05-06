using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using NSubstitute;
using Shouldly;
using Xunit;
#pragma warning disable 618

namespace DbUp.Tests.Engine
{
    public class VariableSubstitutionPreprocessorTests
    {
        [Fact]
        public void substitutes_variables_in_body()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "something $somevar$ something")
                .JournalTo(journal)
                .WithVariable("somevar", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            command.CommandText.ShouldBe("something coriander something");
        }

        [Fact]
        public void substitutes_variables_in_quoted_text()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "'$somevar$'")
                .JournalTo(journal)
                .WithVariable("somevar", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            command.CommandText.ShouldBe("'coriander'");
        }

        [Fact]
        public void ignores_undefined_variables_in_comments()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "/*$somevar$*/")
                .JournalTo(journal)
                .WithVariable("beansprouts", "coriander")
                .Build();

            upgradeEngine.PerformUpgrade();

            command.CommandText.ShouldBe("/*$somevar$*/");
        }

        [Fact]
        public void ignores_undefined_variables_in_complex_comments()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var commands = new List<IDbCommand>();

            _ = connection.CreateCommand()
                          .Returns(ci =>
                          {
                              var command = Substitute.For<IDbCommand>();
                              commands.Add(command);
                              return command;
                          });

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "/*/**/$somevar$*/")
                .JournalTo(journal)
                .WithVariable("beansprouts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            commands.ShouldNotBeNull();
            commands.ShouldNotBeEmpty();
            commands[0].CommandText.ShouldBe("IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Db') Exec('CREATE SCHEMA [Db]')");
            commands[1].CommandText.ShouldBe("/*/**/$somevar$*/");
        }

        [Fact]
        public void ignores_undefined_variable_in_line_comment()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "--$somevar$")
                .JournalTo(journal)
                .WithVariable("beansprouts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            result.Successful.ShouldBeTrue();
            command.CommandText.ShouldBe("--$somevar$");
        }

        [Fact]
        public void throws_for_undefined_variable()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "$somevar$")
                .JournalTo(journal)
                .WithVariable("beansprouts", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            result.Successful.ShouldBeFalse();
            result.Error.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void ignores_if_whitespace_between_dollars()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "$some var$")
                .JournalTo(journal)
                .WithVariable("some var", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            result.Successful.ShouldBeTrue();
            command.CommandText.ShouldBe("$some var$");
        }

        [Fact]
        public void ignores_if_newline_between_dollars()
        {
            var journal = Substitute.For<IJournal>();
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(new SubstitutedConnectionConnectionManager(connection), "Db")
                .WithScript("testscript", "$some\nvar$")
                .JournalTo(journal)
                .WithVariable("somevar", "coriander")
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            result.Successful.ShouldBeTrue();
        }
    }
}
