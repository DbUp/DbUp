using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.Support.SQLite
{
    public class SQLiteObjectParser
    {
        /// <summary>
        /// Quotes the name of the SQLite object in square brackets to allow Special characters in the object name.
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <returns>The quoted object name with trimmed whitespace</returns>
        public static string QuoteSqlObjectName(string objectName)
        {
            return QuoteSqlObjectName(objectName, ObjectNameOptions.Trim);
        }

        /// <summary>
        /// Quotes the name of the SQLite object in square brackets to allow Special characters in the object name.
        /// </summary>
        /// <param name="objectName">Name of the object to quote.</param>
        /// <param name="objectNameOptions">The settings which indicate if the whitespace should be dropped or not.</param>
        /// <returns>The quoted object name</returns>
        public static string QuoteSqlObjectName(string objectName, ObjectNameOptions objectNameOptions)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            if (ObjectNameOptions.Trim == objectNameOptions)
                objectName = objectName.Trim();

            // The ] in the string need to be doubled up so it means we always need an un-even number of ]
            if (objectName.StartsWith("[") && objectName.EndsWith("]") && objectName.Count(x => x == ']') % 2 == 1)
                return objectName;

            return string.Concat("[", objectName.Replace("]", "]]"), "]");
        }
    }
}
