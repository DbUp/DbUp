using System;

namespace DbUp.Tests
{
    class TestSqlException : Exception
    {
        public TestSqlException()
        {
        }

        public TestSqlException(string message) : base(message)
        {
        }

        public TestSqlException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
