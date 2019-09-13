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
    }
}
