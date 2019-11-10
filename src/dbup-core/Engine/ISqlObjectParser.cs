using DbUp.Support;

namespace DbUp.Engine
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions.
    /// </summary>
    public interface ISqlObjectParser
    {
        /// <summary>
        /// Quotes the SQL object/identifier to allow Special characters in the object name.       
        /// </summary>
        /// <param name="objectName">Name of the object / identifier to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        string QuoteIdentifier(string objectName);

        /// <summary>
        /// Unquotes the name of the SQL object/identifier from quotes.      
        /// </summary>
        /// <param name="objectName">Name of the object to unquote.</param>
        /// <returns>The unquoted object name</returns>
        string UnquoteIdentifier(string objectName);

        /// <summary>
        /// Quotes the SQL object/identifier to allow Special characters in the object name.      
        /// </summary>
        /// <param name="objectName">Name of the object / identifier to quote.</param>
        /// <param name="objectNameOptions">The settings which indicate if the whitespace should be dropped or not.</param>
        /// <returns>The quoted object name</returns>
        string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions);
    }
}
