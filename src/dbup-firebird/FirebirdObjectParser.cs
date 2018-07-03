using DbUp.Support;

namespace DbUp.Firebird
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class FirebirdObjectParser : SqlObjectParser
    {
        public FirebirdObjectParser() : base("\"", "\"")
        {
        }
    }
}
