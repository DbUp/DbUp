using System;
using System.Linq;

namespace DbUp.Support.SqlServer
{
    internal class SqlObjectParser
    {
        internal static string QuoteSqlObjectName(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            if (objectName.Length > 128)
                throw new ArgumentOutOfRangeException(@"objectName", "A SQL server object name is maximum 128 characters long");

            // The ] in the string need to be doubled up so it means we always need an un-even number of ]
            if (objectName.TrimStart().StartsWith("[") && objectName.TrimEnd().EndsWith("]") && objectName.Count(x => x == ']') % 2 == 1)
                return objectName.Trim();

            return string.Concat("[", objectName.Replace("]", "]]").Trim(), "]");
        }

    }
}
