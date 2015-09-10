using System;
using System.Text;
using DbUp.Support.SqlServer;

namespace DbUp.MySql
{
    /// <summary>
    /// Reads MySQL commands from an underlying text stream. Supports DELIMITED .. statement
    /// </summary>
    public class MySqlCommandReader : SqlCommandReader
    {
        private const string DelimiterKeyword = "DELIMITER";

        /// <summary>
        /// Creates an instance of MySqlCommandReader
        /// </summary>
        public MySqlCommandReader(string sqlText) : base(sqlText, ";", delimiterRequiresWhitespace: false)
        {
        }

        /// <summary>
        /// Hook to support custom statements
        /// </summary>
        protected override bool IsCustomStatement
        {
            get
            {
                string statement;
                return TryPeek(DelimiterKeyword.Length, out statement) &&
                       string.Equals(DelimiterKeyword, statement, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Read a custom statement
        /// </summary>
        protected override void ReadCustomStatement()
        {
            // Move past Delimiter keyword
            var count = DelimiterKeyword.Length + 1;
            Read(new char[count], 0, count);

            SkipWhitespace();
            // Read until we hit the end of line.
            var delimiter = new StringBuilder();
            do
            {
                delimiter.Append(CurrentChar);
                if (Read() == FailedRead)
                {
                    break;
                }
            }
            while (!IsEndOfLine && !IsWhiteSpace);

            Delimiter = delimiter.ToString();
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                Read();
            }
        }
    }
}