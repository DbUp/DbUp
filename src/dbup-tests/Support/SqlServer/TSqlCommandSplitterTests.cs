using System;
using System.Linq;
using System.Text;
using DbUp.SqlServer;
using DbUp.Support;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.SqlServer
{
    public class TSqlCommandSplitterTests
    {
        readonly SqlCommandSplitter sut;

        public TSqlCommandSplitterTests()
        {
            sut = new TSqlCommandSplitter();
        }

        [Fact]
        public void does_not_split_go_in_column_name()
        {
            var statement = @"CREATE PROCEDURE dbo.GetDetails

    @AccountId uniqueidentifier

AS
BEGIN

SELECT AccountId,
        EstimatedInCents,
        OccupationInCents,
        GovernmentInCents
FROM Nowhere
END".Replace("\r\n", "\n");

            var commands = sut.SplitScriptIntoCommands(statement).ToArray();

            commands.Count().ShouldBe(1);
            commands[0].ShouldBe(statement);
        }

        [Fact]
        public void should_treat_bracketed_text_as_single_item()
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT 1 as [']");
            sb.AppendLine("GO");
            sb.AppendLine("Select 2");
            var commands = sut.SplitScriptIntoCommands(sb.ToString());
            commands.Count().ShouldBe(2);
        }

        [Fact]
        public void multiple_brackets_should_escape_properly()
        {
            var sql = "Select 1 as [[a]][b]][c]]]";
            var commands = sut.SplitScriptIntoCommands(sql);
            commands.Count().ShouldBe(1);
        }

        [Fact]
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
            sqlBuilder.AppendLine("INSERT INTO [Foo] ([Text])");
            sqlBuilder.AppendLine("VALUES (N'Some text. /*Strangely Emphasised Text*/ More text')");

            var strangeInsert = sqlBuilder.ToString();
            sqlBuilder.Clear();

            // Combine into one SQL statement separated with GO.
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
            sqlCommands.ShouldNotBeNull();
            sqlCommands.Length.ShouldBe(4);

            sqlCommands[0].ShouldBe(sqlCommandWithMultiLineComment.Trim());
            sqlCommands[1].ShouldBe(sqlCommandWithSingleLineComment.Trim());
            sqlCommands[2].ShouldBe($";{Environment.NewLine}{sqlCommandWithSingleLineCommentWithEndDashes.Trim()}");
            sqlCommands[3].ShouldBe(strangeInsert.Trim());
        }
    }
}
