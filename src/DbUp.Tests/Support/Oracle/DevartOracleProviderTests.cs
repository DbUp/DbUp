using System;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Oracle;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests.Support.Oracle
{
    [TestFixture]
    public class DevartOracleProviderTests
    {
        [Test]
        public void execute_splits_scripts_into_batches_seperated_by_a_forward_slash_on_own_line()
        {
            const string script = @"CREATE TABLE BLAH;
                                  /
                                  CREATE TABLE FOO;
                                  /
                                  CREATE TABLE BAR;";

            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            connection.CreateCommand().Returns(command);
            var scriptExecutor = new ScriptExecutor(() => new OracleTestConnectionManager(connection, true), () => logger, () => false, null);

            // Act
            scriptExecutor.Execute(new SqlScript("Test", script));

            // Assert
            command.Received(3).ExecuteNonQuery();
        }

        [Test]
        public void execute_ignores_empty_batches()
        {
            const string script = @"CREATE TABLE BLAH;
                                    /
                                    CREATE TABLE FOO;
                                    /
                                    
                                    /
                                    CREATE TABLE BAR;";

            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var logger = Substitute.For<IUpgradeLog>();

            connection.CreateCommand().Returns(command);

            var scriptExecutor = new ScriptExecutor(() => new OracleTestConnectionManager(connection, true), () => logger, () => false, null);

            // Act
            scriptExecutor.Execute(new SqlScript("Test", script));

            // Assert
            command.Received(3).ExecuteNonQuery();
        }
    }
}
