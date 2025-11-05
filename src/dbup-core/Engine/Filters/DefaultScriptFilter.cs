using System.Collections.Generic;
using System.Linq;
using DbUp.Support;

namespace DbUp.Engine.Filters;

/// <summary>
/// Default script filter that excludes already executed scripts unless they are marked as RunAlways.
/// </summary>
public class DefaultScriptFilter : IScriptFilter
{
    /// <inheritdoc/>
    public IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames, ScriptNameComparer comparer)
        => sorted.Where(s => s.SqlScriptOptions.ScriptType == ScriptType.RunAlways || !executedScriptNames.Contains(s.Name, comparer));
}
