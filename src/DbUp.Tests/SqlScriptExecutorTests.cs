using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests
{
    [TestFixture]
    public class SqlScriptExecutorTests
    {
        [Test]
        public void verify_schema_should_not_check_when_schema_is_null()
        {
            var executor = new SqlScriptExecutor(()=>Substitute.For<IConnectionManager>(), () => null, null, () => false, null);

            executor.VerifySchema();
        }

        [Test]
        public void when_schema_is_null_schema_is_stripped_from_scripts()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(()=>new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null);

            executor.Execute(new SqlScript("Test", "create $schema$.Table"));

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create Table", command.CommandText);
        }

        [Test]
        public void uses_variable_subtitute_preprocessor_when_running_scripts()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(()=>new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null);

            executor.Execute(new SqlScript("Test", "create $foo$.Table"), new Dictionary<string, string>{{"foo", "bar"}});

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create bar.Table", command.CommandText);
        }
        
        [Test]
        public void does_not_use_variable_subtitute_preprocessor_when_setting_false()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(()=>new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => false, null);

            executor.Execute(new SqlScript("Test", "create $foo$.Table"), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create $foo$.Table", command.CommandText);
        }

        [Test]
        public void uses_variable_subtitutes_schema()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(()=>new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), "foo", () => true, null);

            executor.Execute(new SqlScript("Test", "create $schema$.Table"));

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("create [foo].Table", command.CommandText);
        }
    }
}
