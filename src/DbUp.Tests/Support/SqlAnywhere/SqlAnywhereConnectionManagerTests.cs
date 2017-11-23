using System;
using System.Linq;
using System.Text;
using DbUp.SqlAnywhere;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.SqlAnywhere
{
    [TestFixture]
    public class SqlAnywhereConnectionManagerTests
    {

        [Test]
        public void CanParseMultilineScript()
        {
            var multiCommand = "CREATE TABLE IF NOT EXISTS 'FOO';";
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE IF NOT EXISTS 'BAR';";
            multiCommand += Environment.NewLine;
            multiCommand += Environment.NewLine;
            multiCommand += "CREATE TABLE IF NOT EXISTS 'BAR_With_Multiple_Lines_Between' {\n Field1, \n Field2};";

            var subject = new SqlAnywhereConnectionManager("connectionstring");
            var result = subject.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(3);
        }

        [Test]
        public void CanParseMultilineScript_When_CommaBetweenBeginAndEnd()
        {
            var multiCommand = @"CREATE TABLE IF NOT EXISTS 'FOO';


                                CREATE TABLE IF NOT EXISTS 'F GO O2';

                                CREATE TABLE IF NOT EXISTS
                                 'FOO3'

                                ;
 
                                CREATE TRIGGER insert_test AFTER INSERT ON test
                                REFERENCING NEW AS new_row
                                FOR EACH ROW
                                BEGIN
                                SET aicol=new_row.keycol;
                                END
                                Go

                                FOR EACH ROW
                                BEGIN
                                SET 2;
                                Another comma;
                                END
                                   go  

                                CREATE TABLE IF NOT EXISTS 'BAR';";

            var subject = new SqlAnywhereConnectionManager("connectionstring");
            var result = subject.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(6);
        }

        [Test]
        public void CanParseMultilineScript_When_SeparatedWithGo()
        {
            var multiCommand = new StringBuilder("CREATE TABLE IF NOT EXISTS 'FOO'");
            multiCommand.AppendLine();
            multiCommand.AppendLine("gO");
            multiCommand.AppendLine("CREATE TABLE IF NOT EXISTS 'BAR'");
            multiCommand.AppendLine();
            multiCommand.AppendLine();
            multiCommand.AppendLine("GO");
            multiCommand.AppendLine("CREATE TABLE IF NOT EXISTS 'BAR_With_Multiple_Lines_Between' {\n Field1, \n Field2}");
            multiCommand.AppendLine("   go   ");

            var subject = new SqlAnywhereConnectionManager("connectionstring");
            var result = subject.SplitScriptIntoCommands(multiCommand.ToString());

            result.Count().ShouldBe(3);
        }
        [Test]
        public void CanParseSingleScriptOnMultipleLines()
        {
            const string singleCommand = "CREATE TABLE IF NOT EXISTS 'FOO'{\n Field1, \n Field2};";

            var subject = new SqlAnywhereConnectionManager("connectionstring");
            var result = subject.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }
    }
}