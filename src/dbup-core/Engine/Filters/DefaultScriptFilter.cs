using System.Collections.Generic;
using System.Linq;
using DbUp.Support;

namespace DbUp.Engine.Filters
{
    public class DefaultScriptFilter : IScriptFilter
    {
        public IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames, ScriptNameComparer comparer)
             => sorted.Where(s => s.SqlScriptOptions.ScriptType == ScriptType.RunAlways || !executedScriptNames.Contains(s.Name, comparer));
    }
}
