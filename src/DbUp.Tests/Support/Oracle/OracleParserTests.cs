using System.Linq;
using DbUp.Oracle;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.Oracle
{
    public class OracleParserTests
    {
        [Test]
        public void should_parse_out_line_comments()
        {
            var testSql = @"Select * FROM -- This is a line comment
                            Employees;";

            var parser = new OracleSqlParser(testSql);
            
            var command = parser.Commands.FirstOrDefault();
            command.ShouldBe(@"Select * FROM Employees");
        }

        [Test]
        public void should_parse_out_block_comments()
        {
            var testSql = @"Select * FROM /* make sure you   
                            you are able to parse a multiline
                            block comment */ Employees;";

            var parser = new OracleSqlParser(testSql);

            var command = parser.Commands.FirstOrDefault();
            command.ShouldBe(@"Select * FROM Employees");
        }

        [Test]
        public void should_reduce_whitespace_to_single_spcae()
        {
            var testSql = " \t \t \t    Select \n \t    *   FROM    Employees;";

            var parser = new OracleSqlParser(testSql);

            var command = parser.Commands.FirstOrDefault();
            command.ShouldBe(@"Select * FROM Employees");
        }

        [Test]
        public void should_remove_semicolon_from_simple_commands()
        {
            var testSql = @"Select * FROM Employees;";

            var parser = new OracleSqlParser(testSql);

            var command = parser.Commands.FirstOrDefault();
            Assert.IsNotNull(command);
            Assert.IsFalse(command.Contains(";"));
        }

        [Test]
        public void should_not_remove_semicolon_from_complex_commands()
        {
            var testSql = @"CREATE OR REPLACE TRIGGER TR_Employees
                               BEFORE INSERT ON Employees
                            FOR EACH ROW
                            BEGIN
                              SELECT SQ_Employees.nextval INTO :new.EmployeeId FROM dual;
                            END;
                            / ";

            var parser = new OracleSqlParser(testSql);
            var command = parser.Commands.FirstOrDefault();
            Assert.IsNotNull(command);
            Assert.IsTrue(command.EndsWith("END;"));
        }

        [Test]
        public void should_not_terminate_for_semicolon_in_begin_end_block()
        {
            var testSql = @"CREATE OR REPLACE TRIGGER TR_Employees
                               BEFORE INSERT ON Employees
                            FOR EACH ROW
                            BEGIN
                              SELECT SQ_Employees.nextval INTO :new.EmployeeId FROM dual;
                            END;
                            / ";

            var parser = new OracleSqlParser(testSql);

            Assert.AreEqual(1, parser.Commands.Count());
        }

        [Test]
        public void should_be_able_to_parse_nested_begin_end_block()
        {
            var testSql = @"CREATE OR REPLACE TRIGGER TR_Employees
                               BEFORE INSERT ON Employees
                            FOR EACH ROW
                            BEGIN
                              SELECT SQ_Employees.nextval INTO :new.EmployeeId FROM dual;
                              BEGIN
                                  Select * FROM Departments;
                                  BEGIN
                                    Select * FROM Employees;
                                  END;
                              END;
                            END;
                            / ";

            var parser = new OracleSqlParser(testSql);

            Assert.AreEqual(1, parser.Commands.Count());
        }
    }
}