using DbUp.Support;

namespace DbUp.Postgresql
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions.
    /// </summary>
    public class PostgresqlObjectParser : SqlObjectParser
    {
        public PostgresqlObjectParser() : base("\"", "\"")
        {
        }
    }
}
