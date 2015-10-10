using System.Linq;
using DbUp.Support;
using Npgsql;

namespace DbUp.Postgresql
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class PostgresqlObjectParser : SqlObjectParser
    {
        public PostgresqlObjectParser() : base(new NpgsqlCommandBuilder())
        {

        }

        public override string QuoteIdentifier(string objectName, ObjectNameOptions objectNameOptions)
        {
            var result = base.QuoteIdentifier(objectName, objectNameOptions);
            // dont quote a quoted identifier.
            result = (objectName.StartsWith("\"") && objectName.EndsWith("\"") && objectName.Count(x => x == '\"') % 2 == 1) ? objectName : result;
            return result;
        }
    }
}
