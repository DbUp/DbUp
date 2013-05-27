using System;
using System.Data;
using System.Data.Common;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Support.Oracle;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification
{
    public class OracleTableJournalTests
    {
        [Test]
        [TestCase("schema", null)]
        [TestCase(null, "table")]
        [TestCase(null, null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void constructing_table_journal_with_null_table_or_schema_parameter_values_throws_argument_null_reference_exception(string schema, string table)
        {
            var connection = Substitute.For<IDbConnection>();
            var logger = Substitute.For<IUpgradeLog>();
            new OracleTableJournal(() => connection, schema, table, logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void constructing_table_journal_with_null_connection_factory_throws_argument_null_reference_exception()
        {
            var logger = Substitute.For<IUpgradeLog>();
            new OracleTableJournal(null, "schema", "table", logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void constructing_table_journal_with_null_logger_throws_argument_null_reference_exception()
        {
            new OracleTableJournal(() => null, "schema", "table", null);
        }

        [Test]
        public void storing_executed_script_checks_if_journal_table_already_exists()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            connection.CreateCommand().Returns(doesTableExistCommand, insertCommand);

            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(doesTableExistCommand.CommandText, Is.EqualTo("SELECT COUNT(*) FROM TEST_DBUP_SCHEMA.TEST_SCHEMA_VERSION_TABLE"));
            doesTableExistCommand.Received().ExecuteScalar();
        }

        [Test]
        public void storing_executed_script_creates_journal_table_if_it_does_not_already_exist()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var createTableCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            connection.CreateCommand().Returns(doesTableExistCommand, createTableCommand, insertCommand);

            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(createTableCommand.CommandText, Is.EqualTo("CREATE TABLE TEST_DBUP_SCHEMA.TEST_SCHEMA_VERSION_TABLE (ID VARCHAR2(32) DEFAULT sys_guid() NOT NULL, SCRIPT_NAME VARCHAR2(255) NOT NULL, APPLIED DATE NOT NULL, CONSTRAINT PK_TEST_SCHEMA_VERSION_TABLE PRIMARY KEY (ID) ENABLE VALIDATE)"));
            createTableCommand.Received().ExecuteNonQuery();
        }

        [Test]
        public void storing_executed_script_in_journal_table_performs_insert_statement()
        {
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var connection = Substitute.For<IDbConnection>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(true);
            connection.CreateCommand().Returns(doesTableExistCommand, insertCommand);

            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(insertCommand.CommandText, Is.StringStarting("INSERT INTO TEST_DBUP_SCHEMA.TEST_SCHEMA_VERSION_TABLE (SCRIPT_NAME, APPLIED) VALUES ('Test', TO_DATE("));
            insertCommand.Received().ExecuteNonQuery();
        }

        [Test]
        public void getting_executed_scripts_checks_if_journal_table_already_exists()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            connection.CreateCommand().Returns(doesTableExistCommand, insertCommand);

            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(doesTableExistCommand.CommandText, Is.EqualTo("SELECT COUNT(*) FROM TEST_DBUP_SCHEMA.TEST_SCHEMA_VERSION_TABLE"));
            doesTableExistCommand.Received().ExecuteScalar();
        }

        [Test]
        public void getting_executed_scripts_writes_informational_log()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            connection.CreateCommand().Returns(command);

            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            logger.Received(1).WriteInformation("Fetching list of already executed scripts.");
        }

        [Test]
        public void getting_executed_scripts_writes_informational_log_if_journal_table_does_not_exist()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();
 
            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            connection.CreateCommand().Returns(doesTableExistCommand);
            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            logger.Received(1).WriteInformation("The TEST_DBUP_SCHEMA.TEST_SCHEMA_VERSION_TABLE table could not be found. The database is assumed to be at version 0.");
        }

        [Test]
        public void getting_executed_scripts_returns_empty_array_if_journal_table_does_not_exist()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            connection.CreateCommand().Returns(doesTableExistCommand);

            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            var result = tableJournal.GetExecutedScripts();

            // Assert
            Assert.That(result.Length, Is.EqualTo(0));
        }

        [Test]
        public void getting_executed_scripts_generates_expected_select_statement()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var selectCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();
            
            doesTableExistCommand.ExecuteScalar().Returns(true);
            connection.CreateCommand().Returns(doesTableExistCommand, selectCommand);
            var tableJournal = new OracleTableJournal(() => connection, "TEST_DBUP_SCHEMA", "TEST_SCHEMA_VERSION_TABLE", logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            Assert.That(selectCommand.CommandText, Is.EqualTo("SELECT SCRIPT_NAME FROM TEST_DBUP_SCHEMA.TEST_SCHEMA_VERSION_TABLE ORDER BY SCRIPT_NAME"));
        }

        [Test]
        public void getting_executed_scripts_uses_default_schema_and_table_names_if_not_provided()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var selectCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();
            
            doesTableExistCommand.ExecuteScalar().Returns(true);
            connection.CreateCommand().Returns(doesTableExistCommand, selectCommand);
            var tableJournal = new OracleTableJournal(() => connection, logger);

            // Act
            tableJournal.GetExecutedScripts();

            // Assert
            Assert.That(
                doesTableExistCommand.CommandText,
                Is.EqualTo(string.Format("SELECT COUNT(*) FROM {0}.{1}", OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName)));

            Assert.That(
                selectCommand.CommandText,
                Is.EqualTo(string.Format("SELECT SCRIPT_NAME FROM {0}.{1} ORDER BY SCRIPT_NAME", OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName)));
        }

        [Test]
        public void storing_executed_scripts_uses_default_schema_and_table_names_if_not_provided()
        {
            var connection = Substitute.For<IDbConnection>();
            var doesTableExistCommand = Substitute.For<IDbCommand>();
            var createTableCommand = Substitute.For<IDbCommand>();
            var insertCommand = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            doesTableExistCommand.ExecuteScalar().Returns(_ => { throw new MockDbException(); });
            connection.CreateCommand().Returns(doesTableExistCommand, createTableCommand, insertCommand);
            var tableJournal = new OracleTableJournal(() => connection, logger);

            // Act
            tableJournal.StoreExecutedScript(new SqlScript("Test", "irrelevant"));

            // Assert
            Assert.That(
                doesTableExistCommand.CommandText,
                Is.EqualTo(string.Format("SELECT COUNT(*) FROM {0}.{1}", OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName)));

            Assert.That(
                createTableCommand.CommandText,
                Is.EqualTo(string.Format("CREATE TABLE {0}.{1} (ID VARCHAR2(32) DEFAULT sys_guid() NOT NULL, SCRIPT_NAME VARCHAR2(255) NOT NULL, APPLIED DATE NOT NULL, CONSTRAINT PK_{1} PRIMARY KEY (ID) ENABLE VALIDATE)", OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName)));

            Assert.That(
                insertCommand.CommandText,
                Is.StringStarting(string.Format("INSERT INTO {0}.{1} (SCRIPT_NAME, APPLIED) VALUES ('Test', TO_DATE(", OracleTableJournal.DefaultSchema, OracleTableJournal.DefaultTableName)));
        }
    }

    public class MockDbException : DbException
    {
    }
}