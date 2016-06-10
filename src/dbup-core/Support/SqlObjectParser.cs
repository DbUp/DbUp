using DbUp.Engine;
using System;
using System.Text.RegularExpressions;

namespace DbUp.Support
{
    public abstract class SqlObjectParser : ISqlObjectParser
    {
        readonly string quotePrefix;
        readonly string quoteSuffix;

        protected SqlObjectParser(string quotePrefix, string quoteSuffix)
        {
            this.quotePrefix = quotePrefix;
            this.quoteSuffix = quoteSuffix;
        }

        /// <summary>
        /// Quotes the name of the SQLite object in square brackets to allow Special characters in the object name.
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        public string QuoteIdentifier(string objectName)
        {
            return QuoteIdentifier(objectName, ObjectNameOptions.Trim);
        }

        /// <summary>
        /// Quotes the name of the SQLite object in square brackets to allow Special characters in the object name.
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <param name="objectNameOptions">The settings which indicate if the whitespace should be dropped or not.</param>
        /// <returns>The quoted object name</returns>
        public virtual string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            if (ObjectNameOptions.Trim == objectNameOptions)
                objectName = objectName.Trim();

            // defer to sqlite command implementation.
            return $"{quotePrefix}{objectName}{quoteSuffix}";
        }

        public virtual string UnquoteIdentifier(string objectName)
        {
            var prefix = Regex.Escape(quotePrefix);
            var suffix = Regex.Escape(quoteSuffix);
            var match = Regex.Match(objectName, $"^({prefix})?(?<unquoted>.*)({suffix})?$");
            return match.Groups["unquoted"].Value;
        }
    }
}
