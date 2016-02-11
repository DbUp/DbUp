using DbUp.Engine;
using System;
using System.Data.Common;
using System.Linq;
using MySql.Data.MySqlClient;
using DbUp.Support;

namespace DbUp.MySql
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class MySqlObjectParser : SqlObjectParser
    {
        public MySqlObjectParser() : base(new MySqlCommandBuilder())
        {

        }

        public override string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions)
        {            
            var result = base.QuoteIdentifier(objectName, objectNameOptions);
            // dont quote a quoted identifier.
            result = (objectName.StartsWith("`") && objectName.EndsWith("`") && objectName.Count(x => x == '`') % 2 == 1) ? objectName : result;
            return result;
        }
    }
}
