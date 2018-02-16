using System.Collections.Generic;
using System.Linq;

namespace DbUp.Engine.Filters
{
    public class DefaultScriptFilter : IScriptFilter
    {
        public IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames)
             =>  sorted.Where(s => !executedScriptNames.Contains(s.Name));
    }
}