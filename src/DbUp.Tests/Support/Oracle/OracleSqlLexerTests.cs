using System.Linq;
using DbUp.Oracle;
using NUnit.Framework;

namespace DbUp.Tests.Support.Oracle
{
    [TestFixture]
    public class OracleSqlLexerTests
    {
        [Test]
        public void should_tokenize_out_line_comments()
        {
            var testSql = @"Select * FROM -- This is a line comment
                            Employees;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql);
            Assert.AreEqual(9, tokens.Count());
        }

        [Test]
        public void should_tokenize_out_block_comments()
        {
            var testSql = @"Select * FROM /* make sure you   
                            you are able to parse a multiline
                            block comment */ Employees;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql);
            Assert.AreEqual(9, tokens.Count());
        }

        [Test]
        public void should_tokenize_semicolon()
        {
            var testSql = @"Select * FROM Employees;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Semicolon");
            Assert.AreEqual(1, tokens.Count());
        }

        [Test]
        public void should_tokenize_asterick_wildcard()
        {
            var testSql = @"Select * FROM Employees;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Wildcard");
            Assert.AreEqual(1, tokens.Count());
        }

        [Test]
        public void should_tokenize_double_quote()
        {
            var testSql = "Select \"NAME\" FROM Employees;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "DoubleQuoted");
            Assert.AreEqual(1, tokens.Count());
        }

        [Test]
        public void should_tokenize_single_quote()
        {
            var testSql = "Select \"Name\" FROM Employees WHERE \"Name\" = 'Bill Lumberg';";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "SingleQuoted");
            Assert.AreEqual(1, tokens.Count());
        }

        [Test]
        public void should_tokenize_a_word()
        {
            var testSql = "Select * FROM Employees;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Word");
            Assert.AreEqual(3, tokens.Count());
        }

        [Test]
        public void should_tokenize_the_equal_sign()
        {
            var testSql = "Select \"Name\" FROM Employees WHERE \"Name\" = 'Bill Lumberg';";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Equals");
            Assert.AreEqual(1, tokens.Count());
        }

        [Test]
        public void should_tokenize_the_punctuation()
        {
            var testSql = @"[{This is a test, (it is only a test).}]|:`";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Punctuation");
            Assert.AreEqual(11, tokens.Count());
        }

        [Test]
        public void should_tokenize_the_whitespace()
        {
            var testSql = "\tSelect\n *\t FROM   Employees  ;";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Whitespace");
            Assert.AreEqual(5, tokens.Count());
        }

        [Test]
        public void should_tokenize_the_slash()
        {
            var testSql = @"CREATE OR REPLACE TRIGGER TR_Employees
                               BEFORE INSERT ON Employees
                            FOR EACH ROW
                            BEGIN
                              SELECT SQ_Employees.nextval INTO :new.EmployeeId FROM dual;
                            END;
                            / ";

            var lexer = new OracleSqlLexer();
            var tokens = lexer.Tokenize(testSql).Where(t => t.Kind.Name == "Slash");
            Assert.AreEqual(1, tokens.Count());
        }
    }
}