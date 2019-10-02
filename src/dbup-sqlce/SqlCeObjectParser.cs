using DbUp.Support;

namespace DbUp.SqlCe
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions.
    /// </summary>
    public class SqlCeObjectParser : SqlObjectParser
    {
        public SqlCeObjectParser() : base("[", "]")
        {
        }
    }
}
