using DbUp.Engine;
using DbUp.Support;
using System;
using System.Linq;
#if MONO
using SQLiteCommandBuilder = Mono.Data.Sqlite.SqliteCommandBuilder;
#else
using System.Data.SQLite;
#endif

namespace DbUp.SQLite
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class SQLiteObjectParser : SqlObjectParser
    {       
        public SQLiteObjectParser() : base(new SQLiteCommandBuilder())
        {
        }

        public override string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions)
        {
            var result = base.QuoteIdentifier(objectName, objectNameOptions);
            // dont quote a quoted identifier.
            result = (objectName.StartsWith("[") && objectName.EndsWith("]") && objectName.Count(x => x == ']') % 2 == 1) ? objectName : result;
            return result;
        }
    }
}
