using System;
using System.Data;
using System.Data.Common;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Oracle;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Oracle
{
    public class OracleTableJournalTests
    {
        private QueryProvider queryProvider = new QueryProvider();
        [Test]
        public void storing_executed_script_checks_if_journal_table_already_exists()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            dbConnection.CreateCommand().Returns(doesTableExistCommand, insertCommand);

            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(doesTableExistCommand.CommandText, Is.EqualTo(queryProvider.VersionTableDoesTableExist()));
            doesTableExistCommand.Received().ExecuteScalar();
        }

        [Test]
        public void storing_executed_script_creates_journal_table_if_it_does_not_already_exist()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var createTableCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            dbConnection.CreateCommand().Returns(doesTableExistCommand, createTableCommand, insertCommand);

            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(createTableCommand.CommandText, Is.EqualTo(queryProvider.VersionTableCreationString()));
            createTableCommand.Received().ExecuteNonQuery();
        }

        [Test]
        public void storing_executed_script_in_journal_table_performs_insert_statement()
        {
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var dbConnection = Substitute.For<IDbConnection>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(true);
            dbConnection.CreateCommand().Returns(doesTableExistCommand, insertCommand);

            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(insertCommand.CommandText, Is.StringStarting(queryProvider.VersionTableNewEntry()));
            insertCommand.Received().ExecuteNonQuery();
        }

        [Test]
        public void storing_part_executed_script_in_journal_table()
        {
            // Given
            var dbConnection = Substitute.For<IDbConnection>();
            var connectionManager = new TestConnectionManager(dbConnection, true);
            var command = Substitute.For<IDbCommand>();
            var param1 = Substitute.For<IDbDataParameter>();
            var param2 = Substitute.For<IDbDataParameter>();
            var param3 = Substitute.For<IDbDataParameter>();
            var param4 = Substitute.For<IDbDataParameter>();
            var param5 = Substitute.For<IDbDataParameter>();
            dbConnection.CreateCommand().Returns(command);
            command.CreateParameter().Returns(param1, param2, param3, param4, param5);
            command.ExecuteScalar().Returns(x => { throw new MockDbException(); });
            var consoleUpgradeLog = new ConsoleUpgradeLog();
            var journal = new TableJournal(() => connectionManager, () => consoleUpgradeLog);

            // When
            journal.StoreExecutedScript(new SqlScript("test", "select 1"));

            // Expect
            command.Received(5).CreateParameter();
            Assert.AreEqual("scriptName", param1.ParameterName);
            Assert.AreEqual("applied", param2.ParameterName);
            Assert.AreEqual("failureStatementIndex", param3.ParameterName);
            Assert.AreEqual("failureRemark", param4.ParameterName);
            Assert.AreEqual("hash", param5.ParameterName);
            command.Received().ExecuteNonQuery();
        }

        [Test]
        public void getting_executed_scripts_checks_if_journal_table_already_exists()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            dbConnection.CreateCommand().Returns(doesTableExistCommand, insertCommand);

            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(doesTableExistCommand.CommandText, Is.EqualTo(queryProvider.VersionTableDoesTableExist()));
            doesTableExistCommand.Received().ExecuteScalar();
        }

        [Test]
        public void getting_executed_scripts_writes_informational_log()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            dbConnection.CreateCommand().Returns(command);

            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            logger.Received(1).WriteInformation("Fetching list of already executed scripts.");
        }

        [Test]
        public void getting_executed_scripts_writes_informational_log_if_journal_table_does_not_exist()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            dbConnection.CreateCommand().Returns(doesTableExistCommand);
            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            logger.Received(1).WriteInformation(String.Format("The {0} table could not be found. The database is assumed to be at version 0.", QueryProvider.VersionTableName));
        }

        [Test]
        public void getting_executed_scripts_returns_empty_array_if_journal_table_does_not_exist()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            dbConnection.CreateCommand().Returns(doesTableExistCommand);

            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            var result = tableJournal.GetExecutedScripts();

            // Assert
            Assert.That(result.Length, Is.EqualTo(0));
        }

        [Test]
        public void getting_executed_scripts_generates_expected_select_statement()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var selectCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(true);
            dbConnection.CreateCommand().Returns(doesTableExistCommand, selectCommand);
            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            Assert.That(selectCommand.CommandText, Is.EqualTo(queryProvider.GetVersionTableExecutedScriptsSql()));
        }

        [Test]
        public void getting_executed_scripts_uses_default_schema_and_table_names_if_not_provided()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var selectCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(true);
            dbConnection.CreateCommand().Returns(doesTableExistCommand, selectCommand);
            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            Assert.That(
                doesTableExistCommand.CommandText,
                Is.EqualTo(queryProvider.VersionTableDoesTableExist()));

            Assert.That(
                selectCommand.CommandText,
                Is.EqualTo(queryProvider.GetVersionTableExecutedScriptsSql()));
        }

        [Test]
        public void storing_executed_scripts_uses_default_schema_and_table_names_if_not_provided()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var createTableCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            dbConnection.CreateCommand().Returns(doesTableExistCommand, createTableCommand, insertCommand);
            var tableJournal = new TableJournal(() => new TestConnectionManager(dbConnection, true), () => logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(
                doesTableExistCommand.CommandText,
                Is.EqualTo(queryProvider.VersionTableDoesTableExist()));

            Assert.That(
                createTableCommand.CommandText,
                Is.EqualTo(queryProvider.VersionTableCreationString()));

            Assert.That(
                insertCommand.CommandText,
                Is.StringStarting(queryProvider.VersionTableNewEntry()));
        }
    }

    public class MockDbException : DbException
    {
    }
}
