using System;
using System.Text.RegularExpressions;
using DbUp.Engine;

namespace DbUp.Support
{
    public abstract class SqlObjectParser : ISqlObjectParser
    {
        readonly string quotePrefix;
        readonly string quoteSuffix;
        readonly Regex matchQuotes;

        protected SqlObjectParser(string quotePrefix, string quoteSuffix)
        {
            this.quotePrefix = quotePrefix;
            this.quoteSuffix = quoteSuffix;

            var prefix = Regex.Escape(quotePrefix);
            var suffix = Regex.Escape(quoteSuffix);
            matchQuotes = new Regex($"^({prefix}){{1}}?(?<unquoted>.*)({suffix}){{1}}$");
        }

        /// <summary>
        /// Quotes the SQL object/identifier to allow Special characters in the object name.
        /// </summary>
        /// <param name="objectName">Name of the object / identifier to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        public string QuoteIdentifier(string objectName)
        {
            return QuoteIdentifier(objectName, ObjectNameOptions.Trim);
        }

        /// <summary>
        /// Quotes the SQL object/identifier to allow Special characters in the object name.
        /// </summary>
        /// <param name="objectName">Name of the object / identifier to quote.</param>
        /// <param name="objectNameOptions">The settings which indicate if the whitespace should be dropped or not.</param>
        /// <returns>The quoted object name</returns>
        public virtual string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException(nameof(objectName));

            if (objectNameOptions == ObjectNameOptions.Trim)
                objectName = objectName.Trim();

            // Don't double quote
            if (matchQuotes.IsMatch(objectName))
                return objectName;

            return $"{quotePrefix}{objectName}{quoteSuffix}";
        }

        public virtual string UnquoteIdentifier(string objectName)
        {
            return matchQuotes.Match(objectName).Groups["unquoted"].Value;
        }
    }
}
