using DbUp.Support;
using System.Data.SqlServerCe;
using System.Linq;

namespace DbUp.SqlCe
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class SqlCeObjectParser : SqlObjectParser
    {
        public SqlCeObjectParser():base(new SqlCeCommandBuilder())
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
