using System;
using System.Linq;
using System.Text;
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
            const string singleCommand = "create table FOO (myid INT NOT NULL);";

            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void CanParseMultilineScript()
        {
            var multiCommand = "create table FOO (myid INT NOT NULL);";
            multiCommand += Environment.NewLine;
            multiCommand += "create table BAR (myid INT NOT NULL);";

            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void ParsesOutBeginningDelimiter()
        {
            const string singleCommand = "select banner as \"oracle version\" from v$version";
            var multiCommand = new StringBuilder()
                .AppendLine("DELIMITER $$")
                .AppendLine(singleCommand + "$$")
                .Append(singleCommand);

            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand.ToString())
                .ToArray();

            result.ShouldBe(new[]
            {
                singleCommand,
                singleCommand
            });
        }

    }
}
