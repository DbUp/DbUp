using DbUp.Support;

namespace DbUp.MySql
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class MySqlObjectParser : SqlObjectParser
    {
        public MySqlObjectParser() : base("`", "`")
        {
        }
    }
}
