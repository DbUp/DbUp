using System;
using DbUp.Support;
using DbUp.Support.SqlServer;
using NUnit.Framework;

namespace DbUp.Tests.Support.SqlServer
{
    [TestFixture]
    public class PostgreObjectParserTests
    {
        [Test]
        public void QuoteSqlObjectName_should_fail_on_large_object_name()
        {
            var objectName = new string('x', 129);

            Assert.That(() => SqlObjectParser.QuoteSqlObjectName(objectName), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void QuoteSqlObjectName_should_fail_on_empty_object_name()
        {
            var objectName = string.Empty;

            Assert.That(() => SqlObjectParser.QuoteSqlObjectName(objectName), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void QuoteSqlObjectName_should_fail_on_null_object_name()
        {
            string objectName = null;

            Assert.That(() => SqlObjectParser.QuoteSqlObjectName(objectName), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void QuoteSqlObjectName_should_not_change_a_quoted_object_name()
        {
            var objectName = "[MyObject]";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName);

            Assert.That(result, Is.EqualTo(objectName));
        }

        [Test]
        public void QuoteSqlObjectName_should_trim_a_quoted_object_name()
        {
            var objectName = "   [MyObject]  ";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName);

            Assert.That(result, Is.EqualTo(objectName.Trim()));
        }

        [Test]
        public void QuoteSqlObjectName_should_quote_an_unquoted_object_name()
        {
            var objectName = "MyObject";
            var quotedObjectName = "[MyObject]";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName);

            Assert.That(result, Is.EqualTo(quotedObjectName));
        }

        [Test]
        public void QuoteSqlObjectName_should_quote_and_trim_an_unquoted_object_name()
        {
            var objectName = "    MyObject   ";
            var quotedObjectName = "[MyObject]";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName);

            Assert.That(result, Is.EqualTo(quotedObjectName));
        }

        [Test]
        public void QuoteSqlObjectName_should_quote_and_escape_closing_brackets_in_object_name()
        {
            var objectName = "MyObject]";
            var quotedObjectName = "[MyObject]]]";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName);

            Assert.That(result, Is.EqualTo(quotedObjectName));
        }

        [Test]
        public void QuoteSqlObjectName_should_quote_and_not_escape_opening_brackets_in_object_name()
        {
            var objectName = "MyO[bject";
            var quotedObjectName = "[MyO[bject]";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName);

            Assert.That(result, Is.EqualTo(quotedObjectName));
        }

        [Test]
        public void QuoteSqlObjectName_without_trim_should_leave_start_and_end_whitespace_intact()
        {
            var objectName = "    MyObject   ";
            var quotedObjectName = "[    MyObject   ]";

            var result = SqlObjectParser.QuoteSqlObjectName(objectName, ObjectNameOptions.None);

            Assert.That(result, Is.EqualTo(quotedObjectName));
        }
    }
}
