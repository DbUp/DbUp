using DbUp.Support;

namespace DbUp.Oracle
{
    public class OracleObjectParser : SqlObjectParser
    {
        public OracleObjectParser() : base("\"", "\"")
        {
        }
    }
}
