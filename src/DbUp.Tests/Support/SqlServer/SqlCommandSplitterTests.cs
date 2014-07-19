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
            var sqlCommandWithMultiLineComment = @"/*
                                                    This is a multi line comment 1.
                                                    GO
                                                    --
                                                    Other comment text
                                                    Go
                                                   */
                                                   INSERT INTO A (GO) VALUES (1);";

            var sqlCommandWithSingleLineComment = @"--Single line Comment no end comment dashes GO
                                                     INSERT INTO A (X) VALUES ('GO');";

            var sqlCommandWithSingleLineCommentWithEndDashes = @"--Single line Comment WITH END comment dashes GO --
                                                                  INSERT INTO A (go) VALUES ('Go');";

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(sqlCommandWithMultiLineComment);
            sqlBuilder.AppendLine(sqlGo);
            sqlBuilder.AppendLine(sqlCommandWithSingleLineComment);
            sqlBuilder.AppendLine(sqlGo);
            sqlBuilder.AppendLine(sqlCommandWithSingleLineCommentWithEndDashes);

            var sqlText = sqlBuilder.ToString();
            Console.WriteLine("===== Splitting the following SQL =============");
            Console.WriteLine(sqlText);
            Console.WriteLine("===============================================");
            var sqlCommands = sut.SplitScriptIntoCommands(sqlText).ToArray();

            foreach (var item in sqlCommands)
            {
                Console.WriteLine("=========== Parsed Command ============");
                Console.WriteLine(item);
                Console.WriteLine("=======================================");
            }
            Assert.That(sqlCommands, Is.Not.Null);
            Assert.That(sqlCommands.Count(), Is.EqualTo(3));

            // I compare the original sql text with the commands but remove whitespace characters as the parser is trimming some whitespace in some instances.
            Assert.That(ExceptBlanks(sqlCommands[0]), Is.EqualTo(ExceptBlanks(sqlCommandWithMultiLineComment)));
            Assert.That(ExceptBlanks(sqlCommands[1]), Is.EqualTo(ExceptBlanks(sqlCommandWithSingleLineComment)));
            Assert.That(ExceptBlanks(sqlCommands[2]), Is.EqualTo(ExceptBlanks(sqlCommandWithSingleLineCommentWithEndDashes)));      
        }

        public static string ExceptBlanks(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

    }
}

