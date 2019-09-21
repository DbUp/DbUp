using System;
using System.Linq;
using System.Text;
using DbUp.MySql;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.MySql
{
    public class MySqlConnectionManagerTests
    {
        [Fact]
        public void CanParseSingleLineScript()
        {
            const string singleCommand = "CREATE TABLE IF NOT EXISTS 'FOO';";

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void CanParseMultilineScript()
        {
            var multiCommand = "CREATE TABLE IF NOT EXISTS 'FOO';";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE IF NOT EXISTS 'BAR';";

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void ParsesOutDelimiter()
        {
            var multiCommand = "USE `test`;";
            multiCommand += "CREATE TABLE IF NOT EXISTS 'FOO';";
            multiCommand += Environment.NewLine;
            multiCommand += "DELIMITER $$";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE 'ZIP'$$";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE IF NOT EXISTS 'BAR';";

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            var enumerable = result as string[] ?? result.ToArray();
            enumerable.Length.ShouldBe(4);
            enumerable[0].IndexOf("DELIMITER", StringComparison.Ordinal).ShouldBe(-1);
            enumerable[1].IndexOf("DELIMITER", StringComparison.Ordinal).ShouldBe(-1);
            enumerable[2].IndexOf("DELIMITER", StringComparison.Ordinal).ShouldBe(-1);
            enumerable[3].IndexOf("DELIMITER", StringComparison.Ordinal).ShouldBe(-1);
        }

        [Fact]
        public void ParsesOutBeginningDelimiter()
        {
            var multiCommand = new StringBuilder()
                .AppendLine("DELIMITER $$")
                .AppendLine("CREATE TABLE 'ZIP'$$")
                .Append("CREATE TABLE IF NOT EXISTS 'BAR';");

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand.ToString())
                .ToArray();

            result.ShouldBe(new[]
            {
                "CREATE TABLE 'ZIP'",
                "CREATE TABLE IF NOT EXISTS 'BAR';"
            });
        }
    }
}