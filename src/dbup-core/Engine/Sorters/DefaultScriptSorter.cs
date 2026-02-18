using System.Collections.Generic;
using System.Linq;
using DbUp.Support;

namespace DbUp.Engine.Sorters;

/// <summary>
/// Default script sorter: by RunGroupOrder then by script name using the configured comparer.
/// </summary>
public class DefaultScriptSorter : IScriptSorter
{
    /// <inheritdoc />
    public IEnumerable<SqlScript> Sort(IEnumerable<SqlScript> scripts, ScriptNameComparer comparer)
        => scripts.OrderBy(s => s.SqlScriptOptions.RunGroupOrder).ThenBy(s => s.Name, comparer);
}
