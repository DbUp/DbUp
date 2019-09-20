using DbUp.Support;

namespace DbUp.Redshift
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class RedshiftObjectParser : SqlObjectParser
    {
        public RedshiftObjectParser() : base("\"", "\"")
        {
        }
    }
}
