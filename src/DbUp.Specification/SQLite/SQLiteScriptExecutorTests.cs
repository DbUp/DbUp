using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Support.SQLite;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace DbUp.Specification.SQLite
{
    [TestFixture]
    public class SQLiteScriptExecutorTests
    {
        [Test]
        public void uses_variable_subtitute_preprocessor_when_running_scripts()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SQLiteScriptExecutor(() => dbConnection, () => new ConsoleUpgradeLog(), () => true, null);

            executor.Execute(new SqlScript("Test", "select * from [table] where foo=$value$"), new Dictionary<string, string> { { "value", "bar" } });

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("select * from [table] where foo=bar", command.CommandText);
        }

        [Test]
        public void does_not_use_variable_subtitute_preprocessor_when_setting_false()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SQLiteScriptExecutor(() => dbConnection, () => new ConsoleUpgradeLog(), () => false, null);

            executor.Execute(new SqlScript("Test", "select * from [table] where foo=$value$"), new Dictionary<string, string> { { "value", "bar" } });

            command.Received().ExecuteNonQuery();
            Assert.AreEqual("select * from [table] where foo=$value$", command.CommandText);
        }
    }
}
