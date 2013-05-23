using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Support.SqlServer;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Specification
{
    [TestFixture]
    public class SqlScriptExecutorTests
    {
        [Test]
        public void verify_schema_should_not_check_when_schema_is_null()
        {
            var executor = new SqlScriptExecutor(() => null, null);

            executor.VerifySchema();
        }

        [Test]
        public void when_schema_is_null_schema_is_stripped_from_scripts()
        {

            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);

            var executor = new SqlScriptExecutor(() => dbConnection, null);
            UpgradeConfiguration configuration = new UpgradeConfiguration {ConnectionFactory = () => dbConnection};

            executor.Execute(new SqlScript("Test", "create $schema$.Table"), configuration);

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create Table", command.CommandText);
        }

        [Test]
        public void uses_variable_subtitute_preprocessor_when_running_scripts()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);

            var executor = new SqlScriptExecutor(() => dbConnection, null);

            UpgradeConfiguration configuration = new UpgradeConfiguration {ConnectionFactory = () => dbConnection, VariablesEnabled = true};
            configuration.AddVariables(new Dictionary<string, string> { { "foo", "bar" } });

            executor.Execute(new SqlScript("Test", "create $foo$.Table"), configuration);

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create bar.Table", command.CommandText);
        }
        
        [Test]
        public void does_not_use_variable_subtitute_preprocessor_when_setting_false()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => dbConnection, null);

            UpgradeConfiguration configuration = new UpgradeConfiguration { ConnectionFactory = () => dbConnection, VariablesEnabled = false };
            configuration.AddVariables(new Dictionary<string, string> { { "foo", "bar" } });

            executor.Execute(new SqlScript("Test", "create $foo$.Table"), configuration);

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create $foo$.Table", command.CommandText);
        }

        [Test]
        public void uses_variable_subtitutes_schema()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);

            var executor = new SqlScriptExecutor(() => dbConnection, "foo");

            UpgradeConfiguration configuration = new UpgradeConfiguration { ConnectionFactory = () => dbConnection, VariablesEnabled = true };
            
            executor.Execute(new SqlScript("Test", "create $schema$.Table"), configuration);

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create [foo].Table", command.CommandText);
        }
    }
}
