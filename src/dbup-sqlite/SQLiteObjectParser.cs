using DbUp.Support;
using System;

namespace DbUp.SQLite
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class SQLiteObjectParser : SqlObjectParser
    {       
        public SQLiteObjectParser() : base("[", "]")
        {
        }
    }
}
