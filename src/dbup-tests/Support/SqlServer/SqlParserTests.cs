using System.Text;
using DbUp.Support;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.SqlServer
{
    public class SqlParserTests
    {
        [Fact]
        public void should_accept_other_delimiters()
        {
            var originalSql = @"something
DELIMITER
go
DELIMITERION;
go;
DELIMITER";
            using (var parser = new TestSqlParser(originalSql, "DELIMITER"))
            {
                var parsedSql = parser.ParseStuff();
                parsedSql.ShouldBe(originalSql);
            }
        }

        [Fact]
        public void should_handle_delimiter_at_the_eof()
        {
            var originalSql = @"something
go";
            using (var parser = new TestSqlParser(originalSql))
            {
                var parsedSql = parser.ParseStuff();
                parsedSql.ShouldBe(originalSql);
            }
        }

        [Fact]
        public void shouldnt_change_parsed_script()
        {
            var originalSql = @"something;
go
someotherthing
GO;
Someotherstuff
gogogo this shouldnt match
;
go";
            using (var parser = new TestSqlParser(originalSql))
            {
                var parsedSql = parser.ParseStuff();
                parsedSql.ShouldBe(originalSql);
            }
        }

        class TestSqlParser : SqlParser
        {
            public TestSqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) : base(sqlText, delimiter, delimiterRequiresWhitespace)
            {
            }

            public string ParseStuff()
            {
                var sb = new StringBuilder();
                ReadCharacter += (type, c) => sb.Append(c);
                Parse();

                return sb.ToString();
            }
        }
    }
}
