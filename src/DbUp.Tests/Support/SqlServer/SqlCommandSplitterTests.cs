using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using NSubstitute;
using NUnit.Framework;
using DbUp.Support.SqlServer;
using System.Text;
using System.Linq;

namespace DbUp.Tests.Support.SqlServer
{

    [TestFixture]
    public class SqlCommandSplitterTests
    {
        private readonly SqlCommandSplitter sut;

        public SqlCommandSplitterTests()
        {
            sut = new SqlCommandSplitter();
        }     

        [Test]
        public void should_split_statements_on_go_and_handle_comments()
        {

            var sqlGo = "GO";
            var sqlGoWithTerminator = "GO;";
            var sqlBuilder = new StringBuilder();

            // Sql command with a multiline comment containing a GO.
            sqlBuilder.AppendLine(@"/*"); // Start of sql comment block.
            sqlBuilder.AppendLine(@"multi line comment 1.");
            sqlBuilder.AppendLine(@"GO");
            sqlBuilder.AppendLine(@"--");
            sqlBuilder.AppendLine(@"Other comment text");
            sqlBuilder.AppendLine(@"Go");
            sqlBuilder.AppendLine(@"*/");  // End of sql comment block.
            sqlBuilder.AppendLine(@"INSERT INTO A (GO) VALUES (1);");

            var sqlCommandWithMultiLineComment = sqlBuilder.ToString();
            sqlBuilder.Clear();

            // Sql command with a single line comment (no end dashes) containing a GO.
            sqlBuilder.AppendLine("--Single line Comment no end comment dashes GO");
            sqlBuilder.AppendLine("INSERT INTO A (X) VALUES ('GO');");
            var sqlCommandWithSingleLineComment = sqlBuilder.ToString();
            sqlBuilder.Clear();

            // Sql command with a single line comment (with end dashes) containing a GO.
            sqlBuilder.AppendLine("--Single line Comment WITH END comment dashes GO --");
            sqlBuilder.AppendLine("INSERT INTO A (go) VALUES ('Go');");

            var sqlCommandWithSingleLineCommentWithEndDashes = sqlBuilder.ToString();
            sqlBuilder.Clear();
            
            // Sql command with a single line comment (with end dashes) containing a GO.
            sqlBuilder.AppendLine("INSERT INTO TABLE [Foo] ([Text)");
            sqlBuilder.AppendLine("VALUES (N'Some text. /*Strangely Emphasised Text*/ More text')");

            var strangeInsert = sqlBuilder.ToString();
            sqlBuilder.Clear();

            // Combine into one SQL statement seperated with GO.          
            sqlBuilder.AppendLine(sqlCommandWithMultiLineComment);
            sqlBuilder.AppendLine(sqlGo);
            sqlBuilder.AppendLine(sqlCommandWithSingleLineComment);
            sqlBuilder.AppendLine(sqlGoWithTerminator);
            sqlBuilder.AppendLine(sqlCommandWithSingleLineCommentWithEndDashes);
            sqlBuilder.AppendLine(sqlGo);
            sqlBuilder.AppendLine(strangeInsert);

            var sqlText = sqlBuilder.ToString();
            Console.WriteLine("===== Splitting the following SQL =============");
            Console.WriteLine(sqlText);
            Console.WriteLine("===============================================");
           
            var commands = sut.SplitScriptIntoCommands(sqlText).ToArray();      

            var sqlCommands = commands;
            foreach (var item in sqlCommands)
            {
                Console.WriteLine("=========== Parsed Command ============");
                Console.WriteLine(item);
                Console.WriteLine("=======================================");
            }
            Assert.That(sqlCommands, Is.Not.Null);
            Assert.That(sqlCommands.Count(), Is.EqualTo(4));

            // I compare the original sql text with the commands but remove whitespace characters as the parser is trimming some whitespace in some instances.
            Assert.That(sqlCommands[0].Trim(), Is.EqualTo(sqlCommandWithMultiLineComment.Trim()));
            Assert.That(sqlCommands[1].Trim(), Is.EqualTo(sqlCommandWithSingleLineComment.Trim()));
            Assert.That(sqlCommands[2].Trim(), Is.EqualTo(sqlCommandWithSingleLineCommentWithEndDashes.Trim()));
            Assert.That(sqlCommands[3].Trim(), Is.EqualTo(strangeInsert.Trim()));
        }       

    }
}

