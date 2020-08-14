using System;
using System.Linq;
using DbUp.Oracle;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.Oracle
{
    public class OracleConnectionManagerTests
    {
        [Fact]
        public void CanParseSingleLineScript()
        {
            const string singleCommand = "create table FOO (myid INT NOT NULL)/";

            var connectionManager = new OracleConnectionManager("connectionstring", new OracleCommandSplitter('/'));
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void CanParseMultilineScript()
        {
            var multiCommand = "create table FOO (myid INT NOT NULL)/";
            multiCommand += Environment.NewLine;
            multiCommand += "create table BAR (myid INT NOT NULL)";

            var connectionManager = new OracleConnectionManager("connectionstring", new OracleCommandSplitter('/'));
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void CanParseWithoutCommentTest()
        {
            var multiCommand = @"
-- inline comment that should be ignored
/*
   multiline comment that should be ignored
*/
create table FOO /*comment*/(text VARCHAR(255) NOT NULL DEFAULT '/*not a comment*/ --not a comment')/ -- inline comment that should be ignored
";
            var connectionManager = new OracleConnectionManager("connectionstring", new OracleCommandSplitter('/', ignoreComments:true));
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(1, "there is more than 1 command");
            result.Single().ShouldBe("create table FOO (text VARCHAR(255) NOT NULL DEFAULT '/*not a comment*/ --not a comment')", "the command not match");
        }
    }
}
