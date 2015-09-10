using System;
using System.Linq;
using DbUp.MySql;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.MySql
{
    [TestFixture]
    public class MySqlConnectionManagerTests
    {
        [Test]
        public void CanParseSingleLineScript()
        {
            const string singleCommand = "CREATE TABLE IF NOT EXISTS 'FOO';";

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }

        [Test]
        public void CanParseMultilineScript()
        {
            var multiCommand = "CREATE TABLE IF NOT EXISTS 'FOO';";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE IF NOT EXISTS 'BAR';";

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(2);
        }

        [Test]
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
    }
}
