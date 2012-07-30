using System;
using System.Linq;

namespace DbUp.Support.SqlServer
{
    internal enum ObjectNameOptions
    {
        /// <summary>
        /// No options are set.
        /// </summary>
        None,

        /// <summary>
        /// Remove starting and ending white space from the object name.
        /// </summary>
        Trim
    }

    internal class SqlObjectParser
    {

        internal static string QuoteSqlObjectName(string objectName)
        {
            return QuoteSqlObjectName(objectName, ObjectNameOptions.Trim);
        }

        internal static string QuoteSqlObjectName(string objectName, ObjectNameOptions trimSettings)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            if (ObjectNameOptions.Trim == trimSettings)
                objectName = objectName.Trim();

            const int SqlSysnameLength = 128;
            if (objectName.Length > SqlSysnameLength)
                throw new ArgumentOutOfRangeException(@"objectName", "A SQL server object name is maximum 128 characters long");

            // The ] in the string need to be doubled up so it means we always need an un-even number of ]
            if (objectName.StartsWith("[") && objectName.EndsWith("]") && objectName.Count(x => x == ']') % 2 == 1)
                return objectName;

            return string.Concat("[", objectName.Replace("]", "]]"), "]");
        }
    }
}
