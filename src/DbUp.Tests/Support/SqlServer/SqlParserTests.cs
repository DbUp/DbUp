using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbUp.Support.SqlServer;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.SqlServer
{
    [TestFixture]
    public class SqlParserTests
    {
        [Test]
        public void should_accept_other_delimiters()
        {
            var originalSql = @"something
DELIMITER
go
DELIMITERION;
go;
DELIMITER";
            var parser = new TestSqlParser(originalSql, "DELIMITER");
            var parsedSql = parser.ParseStuff();
            parsedSql.ShouldBe(originalSql);
        }
        [Test]
        public void should_handle_delimiter_at_the_eof()
        {
            var originalSql = @"something
go";
            var parser = new TestSqlParser(originalSql);
            var parsedSql = parser.ParseStuff();
            parsedSql.ShouldBe(originalSql);
        }
        [Test]
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
            var parser = new TestSqlParser(originalSql);
            var parsedSql = parser.ParseStuff();
            parsedSql.ShouldBe(originalSql);
        }

        private class TestSqlParser : SqlParser
        {
            public TestSqlParser(string sqlText, string delimiter = "GO", bool delimiterRequiresWhitespace = true) : base(sqlText, delimiter, delimiterRequiresWhitespace)
            {
            }

            public string ParseStuff()
            {
                var sb = new StringBuilder();
                this.ReadCharacter += (type, c) => sb.Append(c);
                this.Parse();

                return sb.ToString();
            }
        }
    }
}
