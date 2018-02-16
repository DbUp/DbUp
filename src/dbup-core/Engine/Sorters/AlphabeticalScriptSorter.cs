using System.Collections.Generic;
using System.Linq;

namespace DbUp.Engine.Sorters
{
    public class AlphabeticalScriptSorter : IScriptSorter
    {
        public IEnumerable<SqlScript> Sort(IEnumerable<SqlScript> filtered)
            => filtered.OrderBy(s => s.Name);
    }
}