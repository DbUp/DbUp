using DbUp.Engine;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DbUp.Support
{
    public abstract class SqlObjectParser : ISqlObjectParser
    {
        private DbCommandBuilder commandBuilder;

        protected SqlObjectParser(DbCommandBuilder commandBuilder)
        {
            this.commandBuilder = commandBuilder;
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
            return commandBuilder.QuoteIdentifier(objectName);
        }

        public virtual string UnquoteIdentifier(string objectName)
        {
            return commandBuilder.UnquoteIdentifier(objectName);
        }
    }

}
