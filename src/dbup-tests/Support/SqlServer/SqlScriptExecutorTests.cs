using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SqlServer;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.SqlServer
{
    public class SqlScriptExecutorTests
    {
        [Fact]
        public void verify_schema_should_not_check_when_schema_is_null()
        {
            var executor = new SqlScriptExecutor(() => Substitute.For<IConnectionManager>(), () => null, null, () => false, null, () => Substitute.For<IJournal>());

            executor.VerifySchema();
        }

        [Fact]
        public void when_schema_is_null_schema_is_stripped_from_scripts()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", "create $schema$.Table"));

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe("create Table");
        }

        [Fact]
        public void uses_variable_subtitute_preprocessor_when_running_scripts()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", "create $foo$.Table"), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe("create bar.Table");
        }

        [Fact]
        public void uses_variable_subtitute_preprocessor_when_running_scripts_with_single_line_comment()
        {
            var oneLineComment = @"--from excel $A$6
                                  create $foo$.Table";
            var oneLineCommentResult = @"--from excel $A$6
                                  create bar.Table";
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", oneLineComment), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe(oneLineCommentResult);
        }

        [Fact]
        public void uses_variable_subtitute_preprocessor_when_running_scripts_with_one_line_comment()
        {
            var oneLineComment = @"/* from excel $A$6 */
                                  create $foo$.Table";
            var oneLineCommentResult = @"/* from excel $A$6 */
                                  create bar.Table";
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", oneLineComment), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe(oneLineCommentResult);
        }

        [Fact]
        public void uses_variable_subtitute_preprocessor_when_running_scripts_with_multi_line_comment()
        {
            var multiLineComment = @"/* 
                                        some comment
                                        from excel $A$6 
                                        some comment
                                      */
                                  create $foo$.Table";
            var multiLineCommentResult = @"/* 
                                        some comment
                                        from excel $A$6 
                                        some comment
                                      */
                                  create bar.Table"; var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", multiLineComment), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe(multiLineCommentResult);
        }

        [Fact]
        public void uses_variable_subtitute_preprocessor_when_running_scripts_with_nested_single_line_comment()
        {
            var multiLineComment = @"/* 
                                        some comment
                                        --from excel $A$6 
                                        some comment
                                      */
                                  create $foo$.Table";
            var multiLineCommentResult = @"/* 
                                        some comment
                                        --from excel $A$6 
                                        some comment
                                      */
                                  create bar.Table"; var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", multiLineComment), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe(multiLineCommentResult);
        }

        [Fact]
        public void uses_variable_subtitute_preprocessor_when_running_scripts_with_nested_comment()
        {
            var multiLineComment = @"/* 
                                        some comment
                                        /* from excel $A$6 */
                                        some comment
                                      */
                                  create $foo$.Table";
            var multiLineCommentResult = @"/* 
                                        some comment
                                        /* from excel $A$6 */
                                        some comment
                                      */
                                  create bar.Table"; var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", multiLineComment), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe(multiLineCommentResult);
        }

        [Fact]
        public void does_not_use_variable_subtitute_preprocessor_when_setting_false()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), null, () => false, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", "create $foo$.Table"), new Dictionary<string, string> { { "foo", "bar" } });

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe("create $foo$.Table");
        }

        [Fact]
        public void uses_variable_subtitutes_schema()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => new ConsoleUpgradeLog(), "foo", () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", "create $schema$.Table"));

            command.Received().ExecuteNonQuery();
            command.CommandText.ShouldBe("create [foo].Table");
        }

        [Fact]
        public void logs_output_when_configured_to()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            dbConnection.CreateCommand().Returns(command);
            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true)
            {
                IsScriptOutputLogged = true
            }, () => new ConsoleUpgradeLog(), "foo", () => true, null, () => Substitute.For<IJournal>());

            executor.Execute(new SqlScript("Test", "create $schema$.Table"));

            command.Received().ExecuteReader();
            command.DidNotReceive().ExecuteNonQuery();
            command.CommandText.ShouldBe("create [foo].Table");
        }

        [Fact]
        public void logs_when_dbexception()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            command.When(x => x.ExecuteNonQuery()).Do(x =>
            {
                var ex = Substitute.For<DbException>();
                ex.Message.Returns("Message with curly braces {0}");
                throw ex;
            });
            dbConnection.CreateCommand().Returns(command);
            var logger = Substitute.For<IUpgradeLog>();
            logger.WhenForAnyArgs(x => x.WriteError(null, null)).Do(x => Console.WriteLine(x.Arg<string>(), x.Arg<object[]>()));

            var executor = new SqlScriptExecutor(() => new TestConnectionManager(dbConnection, true), () => logger, null, () => true, null, () => Substitute.For<IJournal>());

            Action exec = () => executor.Execute(new SqlScript("Test", "create $schema$.Table"));
            exec.ShouldThrow<DbException>();
            command.Received().ExecuteNonQuery();
            logger.ReceivedWithAnyArgs().WriteError("", null);
        }
    }
}
