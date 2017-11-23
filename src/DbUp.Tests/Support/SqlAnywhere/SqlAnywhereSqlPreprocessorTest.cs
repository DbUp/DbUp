using System;
using DbUp.SqlAnywhere;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.SqlAnywhere
{
    [TestFixture]
    public class SqlAnywhereSqlPreprocessorTest
    {
        [Test]
        public void Process_Should_ReplaceIdentityFieldCreationWithoutAnyLengthSpecified()
        {
            var subject = new SqlAnywhereSqlPreprocessor();

            var result = subject.Process("[Id] int identity(1,1) not null constraint [PK_schemaversions_Id] primary key,");

            result.ShouldBe("[Id] int identity not null constraint [PK_schemaversions_Id] primary key,");
        }

        [Test]
        public void Process_Should_ReplaceNamedParameterTag()
        {
            var subject = new SqlAnywhereSqlPreprocessor();

            var result = subject.Process("INSERT INTO X(a,b) VALUES (@param1, @param2)");

            result.ShouldBe("INSERT INTO X(a,b) VALUES (:param1, :param2)");
        }
    }
}