using System;
using DbUp.SqlServer;
using DbUp.Support;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.SqlServer
{
    public class SqlObjectParserTests
    {
        readonly SqlServerObjectParser sut;

        public SqlObjectParserTests()
        {
            sut = new SqlServerObjectParser();
        }

        [Fact]
        public void QuoteSqlObjectName_should_fail_on_large_object_name()
        {
            var objectName = new string('x', 129);

            Should.Throw<ArgumentOutOfRangeException>(() => sut.QuoteIdentifier(objectName));
        }

        [Fact]
        public void QuoteSqlObjectName_should_fail_on_empty_object_name()
        {
            var objectName = string.Empty;

            Should.Throw<ArgumentNullException>(() => sut.QuoteIdentifier(objectName));
        }

        [Fact]
        public void QuoteSqlObjectName_should_fail_on_null_object_name()
        {
            string objectName = null;

            Should.Throw<ArgumentNullException>(() => sut.QuoteIdentifier(objectName));
        }

        [Fact]
        public void QuoteSqlObjectName_should_not_change_a_quoted_object_name()
        {
            var objectName = "[MyObject]";

            var result = sut.QuoteIdentifier(objectName);

            result.ShouldBe(objectName);
        }

        [Fact]
        public void QuoteSqlObjectName_should_trim_a_quoted_object_name()
        {
            var objectName = "   [MyObject]  ";

            var result = sut.QuoteIdentifier(objectName);

            result.ShouldBe(objectName.Trim());
        }

        [Fact]
        public void QuoteSqlObjectName_should_quote_an_unquoted_object_name()
        {
            var objectName = "MyObject";
            var quotedObjectName = "[MyObject]";

            var result = sut.QuoteIdentifier(objectName);

            result.ShouldBe(quotedObjectName);
        }

        [Fact]
        public void QuoteSqlObjectName_should_quote_and_trim_an_unquoted_object_name()
        {
            var objectName = "    MyObject   ";
            var quotedObjectName = "[MyObject]";

            var result = sut.QuoteIdentifier(objectName);

            result.ShouldBe(quotedObjectName);
        }

        [Fact]
        public void QuoteSqlObjectName_should_quote_and_escape_closing_brackets_in_object_name()
        {
            var objectName = "MyObject]";
            var quotedObjectName = "[MyObject]]]";

            var result = sut.QuoteIdentifier(objectName);

            result.ShouldBe(quotedObjectName);
        }

        [Fact]
        public void QuoteSqlObjectName_should_quote_and_not_escape_opening_brackets_in_object_name()
        {
            var objectName = "MyO[bject";
            var quotedObjectName = "[MyO[bject]";

            var result = sut.QuoteIdentifier(objectName);

            result.ShouldBe(quotedObjectName);
        }

        [Fact]
        public void QuoteSqlObjectName_without_trim_should_leave_start_and_end_whitespace_intact()
        {
            var objectName = "    MyObject   ";
            var quotedObjectName = "[    MyObject   ]";

            var result = sut.QuoteIdentifier(objectName, ObjectNameOptions.None);

            result.ShouldBe(quotedObjectName);
        }
    }
}
