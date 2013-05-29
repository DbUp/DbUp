using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Support.Oracle;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification
{
    [TestFixture]
    public class OracleScriptExecutorTests
    {
        [Test]
        public void uses_variable_substitute_preprocessor_when_running_scripts()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();

            connection.CreateCommand().Returns(command);

            var executor = new OracleScriptExecutor(() => connection, () => new ConsoleUpgradeLog(), () => true, null);
            var variables = new Dictionary<string, string> { { "columnDefault", "NULL" } };

            // Act
            executor.Execute(new SqlScript("Test", "CREATE TABLE1(COL1 NUMBER DEFAULT $columnDefault$)"), variables);

            // Assert
            Assert.That(command.CommandText, Is.EqualTo("CREATE TABLE1(COL1 NUMBER DEFAULT NULL)"));
            command.Received().ExecuteNonQuery();
        }

        [Test]
        public void does_not_use_variable_substitute_preprocessor_when_setting_false()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();

            connection.CreateCommand().Returns(command);

            var executor = new OracleScriptExecutor(() => connection, () => new ConsoleUpgradeLog(), () => false, null);
            var variables = new Dictionary<string, string> { { "columnDefault", "NULL" } };

            // Act
            executor.Execute(new SqlScript("Test", "CREATE TABLE1(COL1 NUMBER DEFAULT $columnDefault$)"), variables);

            // Assert
            Assert.That(command.CommandText, Is.EqualTo("CREATE TABLE1(COL1 NUMBER DEFAULT $columnDefault$)"));
            command.Received().ExecuteNonQuery();
        }

        [Test]
        public void uses_variable_substitutes()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();

            connection.CreateCommand().Returns(command);

            var scriptExecutor = new OracleScriptExecutor(() => connection, () => new ConsoleUpgradeLog(), () => true, null);
            var variables = new Dictionary<string, string> { { "variable1", "SCHEMA" } };

            // Act
            scriptExecutor.Execute(new SqlScript("Test", "create $variable1$.Table"), variables);

            // Assert
            Assert.That(command.CommandText, Is.EqualTo("create SCHEMA.Table"));
            command.Received().ExecuteNonQuery();
        }

        [Test]
        public void execute_splits_scripts_into_batches_seperated_by_a_forward_slash_on_own_line()
        {
            const string script = @"CREATE TABLE BLAH
/
CREATE TABLE FOO
/
CREATE TABLE BAR";

            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            connection.CreateCommand().Returns(command);

            var scriptExecutor = new OracleScriptExecutor(() => connection, () => logger, () => false, null);

            // Act
            scriptExecutor.Execute(new SqlScript("Test", script));

            // Assert
            command.Received(3).ExecuteNonQuery();
        }

        [Test]
        public void execute_ignores_empty_batches()
        {
            const string script = @"CREATE TABLE BLAH
/
CREATE TABLE FOO
/

/
CREATE TABLE BAR";

            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();
            
            connection.CreateCommand().Returns(command);

            var scriptExecutor = new OracleScriptExecutor(() => connection, () => logger, () => false, null);

            // Act
            scriptExecutor.Execute(new SqlScript("Test", script));

            // Assert
            command.Received(3).ExecuteNonQuery();
        }
    }
}