using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Oracle;
using DbUp.Oracle.Engine;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Support.Oracle
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

            var executor = new ScriptExecutor(() => new OracleTestConnectionManager(connection, true), () => new ConsoleUpgradeLog(), () => true, null);
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

            var executor = new ScriptExecutor(() => new OracleTestConnectionManager(connection, true), () => new ConsoleUpgradeLog(), () => true, null);
            var variables = new Dictionary<string, string> { { "columnDefault", "NULL" } };

            // Act
            executor.Execute(new SqlScript("Test", "CREATE TABLE1(COL1 NUMBER DEFAULT $columnDefault$)"), variables);

            // Assert
            Assert.That(command.CommandText, Is.EqualTo("CREATE TABLE1(COL1 NUMBER DEFAULT NULL)"));
            command.Received().ExecuteNonQuery();
        }

        [Test]
        public void uses_variable_substitutes()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();

            connection.CreateCommand().Returns(command);

            var executor = new ScriptExecutor(() => new OracleTestConnectionManager(connection, true), () => new ConsoleUpgradeLog(), () => true, null);
            var variables = new Dictionary<string, string> { { "variable1", "SCHEMA" } };

            // Act
            executor.Execute(new SqlScript("Test", "create $variable1$.Table"), variables);

            // Assert
            Assert.That(command.CommandText, Is.EqualTo("create SCHEMA.Table"));
            command.Received().ExecuteNonQuery();
        }
    }
}
