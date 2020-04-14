using System.Collections.Generic;
using System.Linq;
using DbUp.Helpers;
using DbUp.Support;

namespace DbUp.Engine
{
    public interface IScriptFilter
    {
        IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames, ScriptNameComparer comparer);
        IEnumerable<SqlScript> Filter(IOrderedEnumerable<SqlScript> sorted, IEnumerable<ExecutedSqlScript> executedScripts, ScriptNameComparer configurationScriptNameComparer, IHasher hasher);
    }
}
