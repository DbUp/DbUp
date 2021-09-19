using System;
using System.Linq;
using DbUp.Support;

namespace DbUp.SqlAnywhere
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class SqlAnywhereObjectParser : SqlObjectParser
    {
        public SqlAnywhereObjectParser() : base("[", "]")
        {
        }

        /// <summary>
        /// Quotes the SqlAnywhere identifier to allow Special characters in the object name.
        /// This function implements System.Data.SqlClient.SqlCommandBuilder.QuoteIdentifier() with an additional
        /// validation which is missing from the SqlCommandBuilder version.
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <param name="objectNameOptions">The settings which indicate if the whitespace should be dropped or not.</param>
        /// <returns>The quoted object name</returns>
        public override string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();


            if (ObjectNameOptions.Trim == objectNameOptions)
                objectName = objectName.Trim();


            const int SqlSysnameLength = 128;
            if (objectName.Length > SqlSysnameLength)
                throw new ArgumentOutOfRangeException(@"objectName", "A SqlAnywhere object name is maximum 128 characters long");


            // The ] in the string need to be doubled up so it means we always need an un-even number of ] 
            if (objectName.StartsWith("[") && objectName.EndsWith("]") && objectName.Count(x => x == ']') % 2 == 1)
                return objectName;


            return string.Concat("[", objectName.Replace("]", "]]"), "]");
        }
    }
}
