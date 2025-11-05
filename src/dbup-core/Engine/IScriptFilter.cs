using System.Collections.Generic;
using DbUp.Support;

namespace DbUp.Engine;

/// <summary>
/// Interface for filtering scripts before execution.
/// </summary>
public interface IScriptFilter
{
    /// <summary>
    /// Filters the scripts based on already executed scripts.
    /// </summary>
    /// <param name="sorted">The sorted list of scripts.</param>
    /// <param name="executedScriptNames">The set of already executed script names.</param>
    /// <param name="comparer">The comparer used to compare script names.</param>
    /// <returns>The filtered list of scripts to execute.</returns>
    IEnumerable<SqlScript> Filter(IEnumerable<SqlScript> sorted, HashSet<string> executedScriptNames, ScriptNameComparer comparer);
}
