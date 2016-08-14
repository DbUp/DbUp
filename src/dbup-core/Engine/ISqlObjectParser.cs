using DbUp.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.Engine
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions.
    /// </summary>
    public interface ISqlObjectParser
    {
        /// <summary>
        /// Quotes the name of the SQL object in square brackets to allow Special characters in the object name.      
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        string QuoteIdentifier(string objectName);

        /// <summary>
        /// Quotes the name of the SQL object in square brackets to allow Special characters in the object name.      
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        string UnquoteIdentifier(string objectName);

        /// <summary>
        /// Quotes the SQL identifier to allow Special characters in the object name.      
        /// </summary>
        /// <param name="objectName">Name of the object / identifier to quote.</param>
        /// <param name="objectNameOptions">The settings which indicate if the whitespace should be dropped or not.</param>
        /// <returns>The quoted object name</returns>
        string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions);
    }
}
