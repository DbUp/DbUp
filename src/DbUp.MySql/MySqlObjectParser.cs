using System;
using DbUp.Support;

namespace DbUp.MySql
{
    class MySqlObjectParser
    {
        public static string QuoteMySqlObjectName(string objectName)
        {
            return QuoteMySqlObjectName(objectName, ObjectNameOptions.Trim);
        }

        public static string QuoteMySqlObjectName(string objectName, ObjectNameOptions objectNameOptions)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            if (ObjectNameOptions.Trim == objectNameOptions)
                objectName = objectName.Trim();

            const int mySqlSysnameLength = 64;
            if(objectName.Length > mySqlSysnameLength)
                throw new ArgumentOutOfRangeException(@"objectName", "A MySql server object name is a maximum 64 characters long");

            return objectName;
        }
    }
}
