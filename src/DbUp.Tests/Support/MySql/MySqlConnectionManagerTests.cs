using System;
using System.Linq;
using DbUp.MySql;
using NUnit.Framework;

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

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CanParseMultilineScript()
        {
            var multiCommand = "CREATE TABLE IF NOT EXISTS 'FOO';";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE IF NOT EXISTS 'BAR';";

            var connectionManager = new MySqlConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            Assert.That(result.Count(), Is.EqualTo(2));
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
            Assert.That(enumerable.Count(), Is.EqualTo(4));
            Assert.That(enumerable[0].IndexOf("DELIMITER", StringComparison.Ordinal), Is.EqualTo(-1));
            Assert.That(enumerable[1].IndexOf("DELIMITER", StringComparison.Ordinal), Is.EqualTo(-1));
            Assert.That(enumerable[2].IndexOf("DELIMITER", StringComparison.Ordinal), Is.EqualTo(-1));
            Assert.That(enumerable[3].IndexOf("DELIMITER", StringComparison.Ordinal), Is.EqualTo(-1));
        }
    }
}
