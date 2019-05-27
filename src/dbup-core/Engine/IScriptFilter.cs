using DbUp.Support;
using System.Collections.Generic;

namespace DbUp.Engine
{
    public interface IScriptFilter
    {
        IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames, ScriptNameComparer comparer);
    }
}