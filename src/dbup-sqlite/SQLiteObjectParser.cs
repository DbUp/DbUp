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
        public SQLiteObjectParser() : base("[", "]")
        {
        }
    }
}
