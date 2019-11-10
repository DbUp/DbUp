using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.Engine
{
    public interface IScriptFilter
    {
        IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames, ScriptNameComparer comparer);
    }
}
